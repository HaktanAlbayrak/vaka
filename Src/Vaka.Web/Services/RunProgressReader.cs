using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.ViewModels.Procedures;
using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed class RunProgressReader : IRunProgressReader
{
    private readonly VakaDbContext _db;
    private readonly IProcedureLinks _links;
    private readonly IDurationCalculator _calculator;
    private readonly ITurkeyClock _clock;

    public RunProgressReader(VakaDbContext db, IProcedureLinks links, IDurationCalculator calculator, ITurkeyClock clock)
    {
        _db = db;
        _links = links;
        _calculator = calculator;
        _clock = clock;
    }

    public async Task<RunProgress> ReadAsync(RunContext context, RunBreakPanel breaks, DateTime openedAtUtc, CancellationToken cancellationToken)
    {
        var rows = await ReadRowsAsync(context.RecordId, cancellationToken);
        var addedAt = await ReadAddedMomentsAsync(context.RecordId, cancellationToken);
        var hasMaterial = !context.MaterialRequired
            || await _db.RecordMaterials.AnyAsync(item => item.CleaningRecordId == context.RecordId && item.RemovedAtUtc == null, cancellationToken);

        var openBreak = breaks.OpenBreak is not null;
        var isActive = context.Status is RecordStatus.Opened or RecordStatus.InProgress;
        var isOwner = context.OwnerUserId == context.ViewerUserId;
        var steps = rows.SelectMany(phase => phase.Steps).ToList();
        var nextIndex = steps.FindIndex(step => step.Status != StepStatus.Completed);
        var anyInProgress = steps.Any(step => step.Status == StepStatus.InProgress);

        var phases = new List<RecordPhaseViewModel>();
        var index = 0;
        RecordStepViewModel? nextModel = null;
        foreach (var phase in rows)
        {
            var models = new List<RecordStepViewModel>();
            foreach (var step in phase.Steps)
            {
                var model = MapStep(step, context, openBreak, isActive, isOwner, anyInProgress, index == nextIndex, index == 0 && !hasMaterial, breaks.Spans, addedAt, openedAtUtc);
                model.IsLastOfPhase = step == phase.Steps[^1];
                if (index == nextIndex)
                {
                    nextModel = model;
                }

                models.Add(model);
                index++;
            }

            phases.Add(MapPhase(phase, [.. models], context, breaks.Spans));
        }

        var canBreak = RunActionRules.CanStartBreak(context, openBreak, anyInProgress, nextModel);
        var reason = canBreak ? null : RunActionRules.BreakBlock(context, openBreak, anyInProgress, nextModel);
        var firstStarter = rows.Count > 0 && rows[0].Steps.Count > 0 ? rows[0].Steps[0].StartedByName : null;
        return new RunProgress([.. phases], canBreak, reason, await ReadSummaryAsync(context.RecordId, cancellationToken), firstStarter);
    }

    private RecordStepViewModel MapStep(
        StepRow step,
        RunContext context,
        bool openBreak,
        bool isActive,
        bool isOwner,
        bool anyInProgress,
        bool isNextIndex,
        bool missingMaterial,
        IReadOnlyList<BreakSpan> spans,
        Dictionary<(int StepId, int UserId), DateTime> addedAt,
        DateTime openedAtUtc)
    {
        var isMember = isOwner || step.Participants.Any(item => item.UserId == context.ViewerUserId);
        var model = new RecordStepViewModel
        {
            Id = step.Id,
            OrderNo = step.OrderNo,
            Title = step.Title,
            Description = step.Description,
            Media = [.. step.Media.Select(media => new StepMediaViewModel
            {
                Id = media.Id,
                MediaType = media.MediaType,
                MediaTypeLabel = ProcedureLabels.MediaType(media.MediaType),
                OriginalFileName = media.OriginalFileName,
                Url = _links.Media(media.Id)
            })],
            Status = step.Status,
            StatusLabel = RecordLabels.Step(step.Status),
            IsNext = isNextIndex && step.Status == StepStatus.Waiting,
            StartedAtLocal = step.StartedAtUtc is { } started ? _clock.ToLocal(started) : null,
            StartedByName = step.StartedByName,
            EndedAtLocal = step.EndedAtUtc is { } ended ? _clock.ToLocal(ended) : null,
            EndedByName = step.EndedByName,
            Participants = [.. step.Participants.Select(item => new StepParticipantViewModel
            {
                UserId = item.UserId,
                FullName = item.FullName,
                RegistryNo = item.RegistryNo,
                IsOwner = item.UserId == context.OwnerUserId,
                AddedAtLocal = _clock.ToLocal(addedAt.GetValueOrDefault((step.Id, item.UserId), openedAtUtc))
            })],
            CanManageParticipants = isOwner && isActive && !openBreak && step.Status == StepStatus.Waiting
        };

        if (step.Status == StepStatus.Waiting)
        {
            model.CannotStartReason = RunActionRules.StartBlock(context, openBreak, isNextIndex, anyInProgress, isMember, missingMaterial);
            model.CanStart = model.CannotStartReason is null;
        }
        else if (step.Status == StepStatus.InProgress)
        {
            model.CannotFinishReason = RunActionRules.FinishBlock(context, openBreak, isMember);
            model.CanFinish = model.CannotFinishReason is null;
            model.LiveElapsedSeconds = _calculator.CalculateLiveElapsed(UtcMoments.Of(step.StartedAtUtc!.Value), context.Now, spans);
        }
        else if (step.Status == StepStatus.Completed)
        {
            model.BreakSeconds = step.BreakSeconds;
            model.DurationSeconds = step.DurationSeconds;
            model.EffortSeconds = step.EffortSeconds;
        }

        return model;
    }

    private RecordPhaseViewModel MapPhase(PhaseRow phase, RecordStepViewModel[] steps, RunContext context, IReadOnlyList<BreakSpan> spans)
    {
        var model = new RecordPhaseViewModel
        {
            Id = phase.Id,
            OrderNo = phase.OrderNo,
            Title = phase.Title,
            MinDurationSeconds = phase.MinDurationSeconds,
            Status = phase.Status,
            StatusLabel = RecordLabels.Phase(phase.Status),
            GrossSeconds = phase.GrossSeconds,
            BreakSeconds = phase.BreakSeconds,
            NetSeconds = phase.NetSeconds,
            BelowMinimum = phase.BelowMinimum,
            BelowMinimumReason = phase.BelowMinimumReason,
            Steps = steps
        };

        if (phase.Status == PhaseStatus.InProgress)
        {
            var start = phase.Steps.Min(step => step.StartedAtUtc)!.Value;
            model.LiveNetSeconds = _calculator.CalculateLiveElapsed(UtcMoments.Of(start), context.Now, spans);
        }

        return model;
    }

    private async Task<List<PhaseRow>> ReadRowsAsync(int recordId, CancellationToken cancellationToken) =>
        await _db.RecordPhases
            .AsNoTracking()
            .Where(phase => phase.CleaningRecordId == recordId)
            .OrderBy(phase => phase.ProcedurePhase.OrderNo)
            .Select(phase => new PhaseRow
            {
                Id = phase.Id,
                OrderNo = phase.ProcedurePhase.OrderNo,
                Title = phase.ProcedurePhase.Title,
                MinDurationSeconds = phase.MinDurationSecondsSnapshot,
                Status = phase.Status,
                GrossSeconds = phase.GrossSeconds,
                BreakSeconds = phase.BreakSeconds,
                NetSeconds = phase.NetSeconds,
                BelowMinimum = phase.BelowMinimum,
                BelowMinimumReason = phase.BelowMinimumReason,
                Steps = phase.Steps
                    .OrderBy(step => step.ProcedureStep.OrderNo)
                    .Select(step => new StepRow
                    {
                        Id = step.Id,
                        OrderNo = step.ProcedureStep.OrderNo,
                        Title = step.ProcedureStep.Title,
                        Description = step.ProcedureStep.Description,
                        Status = step.Status,
                        StartedAtUtc = step.StartedAtUtc,
                        StartedByName = step.StartedBy == null ? null : step.StartedBy.FullName,
                        EndedAtUtc = step.EndedAtUtc,
                        EndedByName = step.EndedBy == null ? null : step.EndedBy.FullName,
                        BreakSeconds = step.BreakSeconds,
                        DurationSeconds = step.DurationSeconds,
                        EffortSeconds = step.EffortSeconds,
                        Media = step.ProcedureStep.Media.OrderBy(media => media.Id).Select(media => new MediaRow(media.Id, media.MediaType, media.OriginalFileName)).ToList(),
                        Participants = step.Participants.OrderBy(item => item.Id).Select(item => new ParticipantRow(item.UserId, item.User.FullName, item.User.RegistryNo)).ToList()
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

    private async Task<Dictionary<(int StepId, int UserId), DateTime>> ReadAddedMomentsAsync(int recordId, CancellationToken cancellationToken)
    {
        var events = await _db.CleaningEvents
            .AsNoTracking()
            .Where(item => item.CleaningRecordId == recordId && item.EventType == EventTypes.ParticipantAdded)
            .OrderBy(item => item.Sequence)
            .Select(item => new { item.PayloadJson, item.OccurredAtUtc })
            .ToListAsync(cancellationToken);

        var moments = new Dictionary<(int, int), DateTime>();
        foreach (var item in events)
        {
            using var document = JsonDocument.Parse(item.PayloadJson);
            moments[(document.RootElement.GetProperty("StepId").GetInt32(), document.RootElement.GetProperty("ParticipantUserId").GetInt32())] = item.OccurredAtUtc;
        }

        return moments;
    }

    private async Task<RecordSummaryViewModel> ReadSummaryAsync(int recordId, CancellationToken cancellationToken)
    {
        var row = await _db.CleaningRecords
            .AsNoTracking()
            .Where(record => record.Id == recordId)
            .Select(record => new { record.StartedAtUtc, record.CompletedAtUtc, record.GrossSeconds, record.BreakSeconds, record.NetSeconds, record.EffortSeconds })
            .FirstAsync(cancellationToken);

        return new RecordSummaryViewModel
        {
            GrossSeconds = row.GrossSeconds,
            BreakSeconds = row.BreakSeconds,
            NetSeconds = row.NetSeconds,
            EffortSeconds = row.EffortSeconds,
            StartedAtLocal = row.StartedAtUtc is { } started ? _clock.ToLocal(started) : null,
            CompletedAtLocal = row.CompletedAtUtc is { } completed ? _clock.ToLocal(completed) : null
        };
    }

    private sealed class PhaseRow
    {
        public int Id { get; init; }
        public int OrderNo { get; init; }
        public string Title { get; init; } = string.Empty;
        public int? MinDurationSeconds { get; init; }
        public PhaseStatus Status { get; init; }
        public int? GrossSeconds { get; init; }
        public int? BreakSeconds { get; init; }
        public int? NetSeconds { get; init; }
        public bool BelowMinimum { get; init; }
        public string? BelowMinimumReason { get; init; }
        public List<StepRow> Steps { get; init; } = [];
    }

    private sealed class StepRow
    {
        public int Id { get; init; }
        public int OrderNo { get; init; }
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }
        public StepStatus Status { get; init; }
        public DateTime? StartedAtUtc { get; init; }
        public string? StartedByName { get; init; }
        public DateTime? EndedAtUtc { get; init; }
        public string? EndedByName { get; init; }
        public int BreakSeconds { get; init; }
        public int? DurationSeconds { get; init; }
        public int? EffortSeconds { get; init; }
        public List<MediaRow> Media { get; init; } = [];
        public List<ParticipantRow> Participants { get; init; } = [];
    }

    private sealed record MediaRow(int Id, StepMediaType MediaType, string OriginalFileName);

    private sealed record ParticipantRow(int UserId, string FullName, string RegistryNo);
}
