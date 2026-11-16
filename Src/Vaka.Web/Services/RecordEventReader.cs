using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed class RecordEventReader : IRecordEventReader
{
    private readonly VakaDbContext _db;
    private readonly ITurkeyClock _clock;
    private readonly IRecordTimelineBuilder _timeline;

    public RecordEventReader(VakaDbContext db, ITurkeyClock clock, IRecordTimelineBuilder timeline)
    {
        _db = db;
        _clock = clock;
        _timeline = timeline;
    }

    public async Task<RecordEventSection> ReadAsync(int recordId, CancellationToken cancellationToken)
    {
        var rows = await _db.CleaningEvents
            .AsNoTracking()
            .Where(item => item.CleaningRecordId == recordId)
            .OrderBy(item => item.Sequence)
            .Select(item => new { item.Sequence, item.EventType, item.PayloadJson, item.ActorUserId, ActorName = item.Actor == null ? null : item.Actor.FullName, item.OccurredAtUtc })
            .ToListAsync(cancellationToken);

        var parsed = rows.Select(row => ParsedEvent.From(row.Sequence, row.EventType, row.ActorUserId, row.OccurredAtUtc, row.PayloadJson)).ToList();
        var userIds = parsed
            .SelectMany(item => (item.Numbers("ParticipantUserIds") ?? []).Append(item.Number("ParticipantUserId") ?? 0).Append(item.Number("SessionUserId") ?? 0).Append(item.ActorUserId ?? 0))
            .Where(id => id != 0)
            .Distinct()
            .ToArray();
        var users = await _db.Users.AsNoTracking().Where(user => userIds.Contains(user.Id)).ToDictionaryAsync(user => user.Id, user => user.FullName, cancellationToken);
        var stepRows = await _db.RecordSteps
            .AsNoTracking()
            .Where(step => step.Phase.CleaningRecordId == recordId)
            .Select(step => new { step.Id, step.ProcedureStep.Title, PhaseOrder = step.Phase.ProcedurePhase.OrderNo, StepOrder = step.ProcedureStep.OrderNo })
            .ToListAsync(cancellationToken);
        var steps = stepRows.ToDictionary(step => step.Id, step => step.Title);
        var stepLabels = stepRows.ToDictionary(step => step.Id, step => $"Faz {step.PhaseOrder} / Adım {step.StepOrder} - {step.Title}");
        var phaseRows = await _db.RecordPhases
            .AsNoTracking()
            .Where(phase => phase.CleaningRecordId == recordId)
            .Select(phase => new { phase.Id, phase.ProcedurePhase.OrderNo })
            .ToListAsync(cancellationToken);
        var phaseOrders = phaseRows.ToDictionary(phase => phase.Id, phase => phase.OrderNo);

        string LocalText(DateTime utc) => _clock.ToLocal(utc).ToString("dd.MM.yyyy HH:mm:ss");
        var events = rows.Select((row, index) => new RecordDetailEventViewModel
        {
            Sequence = row.Sequence,
            OccurredAtLocal = _clock.ToLocal(row.OccurredAtUtc),
            ActorName = row.ActorName ?? "Sistem",
            EventLabel = RecordEventText.Label(row.EventType),
            Summary = RecordEventText.Summary(parsed[index], users, steps, LocalText)
        }).ToArray();
        return new RecordEventSection(events, _timeline.Build(parsed, new TimelineNames(users, stepLabels, phaseOrders)));
    }
}
