using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;
using Vaka.Web.ViewModels.Procedures;
using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed class RecordPhaseReader : IRecordPhaseReader
{
    private readonly VakaDbContext _db;
    private readonly IProcedureLinks _links;
    private readonly ITurkeyClock _clock;

    public RecordPhaseReader(VakaDbContext db, IProcedureLinks links, ITurkeyClock clock)
    {
        _db = db;
        _links = links;
        _clock = clock;
    }

    public async Task<PhaseSection> ReadAsync(int recordId, int ownerUserId, DateTime openedAtUtc, CancellationToken cancellationToken)
    {
        var phases = await _db.RecordPhases
            .AsNoTracking()
            .Where(phase => phase.CleaningRecordId == recordId)
            .OrderBy(phase => phase.ProcedurePhase.OrderNo)
            .Select(phase => new
            {
                phase.ProcedurePhase.OrderNo,
                phase.ProcedurePhase.Title,
                phase.MinDurationSecondsSnapshot,
                phase.Status,
                phase.GrossSeconds,
                phase.BreakSeconds,
                phase.NetSeconds,
                phase.BelowMinimum,
                phase.BelowMinimumReason,
                Steps = phase.Steps
                    .OrderBy(step => step.ProcedureStep.OrderNo)
                    .Select(step => new
                    {
                        step.Id,
                        step.ProcedureStep.OrderNo,
                        step.ProcedureStep.Title,
                        step.ProcedureStep.Description,
                        Media = step.ProcedureStep.Media.OrderBy(media => media.Id)
                            .Select(media => new { media.Id, media.MediaType, media.OriginalFileName }).ToList(),
                        step.Status,
                        step.StartedAtUtc,
                        StartedByName = step.StartedBy == null ? null : step.StartedBy.FullName,
                        step.EndedAtUtc,
                        EndedByName = step.EndedBy == null ? null : step.EndedBy.FullName,
                        step.BreakSeconds,
                        step.DurationSeconds,
                        step.EffortSeconds,
                        Participants = step.Participants.OrderBy(item => item.Id)
                            .Select(item => new
                            {
                                item.UserId,
                                item.User.FullName,
                                item.User.RegistryNo,
                                AddedByName = item.AddedBy.FullName,
                                item.ConfirmedAtUtc,
                                item.IsWorking,
                                HasInterval = item.WorkInterval != null,
                                IntervalStartedAtUtc = item.WorkInterval == null ? null : (DateTime?)item.WorkInterval.StartedAtUtc,
                                IntervalEndedAtUtc = item.WorkInterval == null ? null : item.WorkInterval.EndedAtUtc,
                                IntervalEndKind = item.WorkInterval == null ? null : item.WorkInterval.EndKind,
                                IntervalWorkSeconds = item.WorkInterval == null ? null : item.WorkInterval.WorkSeconds
                            }).ToList()
                    }).ToList()
            })
            .ToListAsync(cancellationToken);

        var addedAt = await ReadAddedMomentsAsync(recordId, cancellationToken);
        var allSteps = phases.SelectMany(phase => phase.Steps).ToList();
        var firstStarted = allSteps.Where(step => step.StartedAtUtc != null).OrderBy(step => step.StartedAtUtc).FirstOrDefault();

        var helpers = allSteps
            .SelectMany(step => step.Participants)
            .Where(item => item.UserId != ownerUserId)
            .DistinctBy(item => item.UserId)
            .Select(item => new RecordHelperViewModel { FullName = item.FullName, RegistryNo = item.RegistryNo })
            .ToArray();

        var model = phases.Select(phase => new RecordDetailPhaseViewModel
        {
            OrderNo = phase.OrderNo,
            Title = phase.Title,
            MinDurationSeconds = phase.MinDurationSecondsSnapshot,
            StatusLabel = RecordLabels.Phase(phase.Status),
            GrossSeconds = phase.GrossSeconds,
            BreakSeconds = phase.BreakSeconds,
            NetSeconds = phase.NetSeconds,
            BelowMinimum = phase.BelowMinimum,
            BelowMinimumReason = phase.BelowMinimumReason ?? string.Empty,
            Steps =
            [
                .. phase.Steps.Select(step => new RecordDetailStepViewModel
                {
                    OrderNo = step.OrderNo,
                    Title = step.Title,
                    Description = step.Description ?? string.Empty,
                    Media =
                    [
                        .. step.Media.Select(media => new StepMediaViewModel
                        {
                            Id = media.Id,
                            MediaType = media.MediaType,
                            MediaTypeLabel = ProcedureLabels.MediaType(media.MediaType),
                            OriginalFileName = media.OriginalFileName,
                            Url = _links.Media(media.Id)
                        })
                    ],
                    StatusLabel = RecordLabels.Step(step.Status),
                    StartedAtLocal = Local(step.StartedAtUtc),
                    StartedByName = step.StartedByName,
                    EndedAtLocal = Local(step.EndedAtUtc),
                    EndedByName = step.EndedByName,
                    BreakSeconds = step.EndedAtUtc is null ? null : step.BreakSeconds,
                    DurationSeconds = step.DurationSeconds,
                    EffortSeconds = step.EffortSeconds,
                    Participants =
                    [
                        .. step.Participants.Select(item =>
                        {
                            var finished = step.Status is StepStatus.Completed or StepStatus.Interrupted;
                            var wholeStep = !item.HasInterval && (item.IsWorking || (finished && !step.Participants.Any(other => other.HasInterval)));
                            var left = item.IntervalEndKind == StepIntervalEndKind.Left;
                            return new RecordDetailParticipantViewModel
                            {
                                FullName = item.FullName,
                                RegistryNo = item.RegistryNo,
                                AddedByName = item.AddedByName,
                                AddedAtLocal = _clock.ToLocal(addedAt.GetValueOrDefault((step.Id, item.UserId), openedAtUtc)),
                                ConfirmedAtLocal = Local(item.ConfirmedAtUtc),
                                HasInterval = !wholeStep,
                                JoinedAtLocal = Local(item.IntervalStartedAtUtc),
                                LeftAtLocal = left ? Local(item.IntervalEndedAtUtc) : null,
                                WorkSeconds = item.HasInterval ? item.IntervalWorkSeconds : (!wholeStep && finished ? 0 : null)
                            };
                        })
                    ]
                })
            ]
        }).ToArray();

        return new PhaseSection(model, firstStarted?.StartedByName, helpers);
    }

    private DateTime? Local(DateTime? utc) => utc is { } value ? _clock.ToLocal(value) : null;

    private async Task<Dictionary<(int StepId, int UserId), DateTime>> ReadAddedMomentsAsync(int recordId, CancellationToken cancellationToken)
    {
        var rows = await _db.CleaningEvents
            .AsNoTracking()
            .Where(item => item.CleaningRecordId == recordId && item.EventType == EventTypes.ParticipantAdded)
            .OrderBy(item => item.Sequence)
            .Select(item => new { item.PayloadJson, item.OccurredAtUtc })
            .ToListAsync(cancellationToken);

        var moments = new Dictionary<(int, int), DateTime>();
        foreach (var row in rows)
        {
            var parsed = ParsedEvent.From(0, EventTypes.ParticipantAdded, null, row.OccurredAtUtc, row.PayloadJson);
            if (parsed.Number("StepId") is { } stepId && parsed.Number("ParticipantUserId") is { } userId)
            {
                moments[(stepId, userId)] = row.OccurredAtUtc;
            }
        }

        return moments;
    }
}
