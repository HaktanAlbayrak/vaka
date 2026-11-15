using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

public sealed class RunCompletion : IRunCompletion
{
    private readonly VakaDbContext _db;
    private readonly ICleaningEventWriter _events;
    private readonly IDurationCalculator _calculator;

    public RunCompletion(VakaDbContext db, ICleaningEventWriter events, IDurationCalculator calculator)
    {
        _db = db;
        _events = events;
        _calculator = calculator;
    }

    public PhaseTotals MeasurePhase(RecordPhase phase, DateTime phaseEnd, IReadOnlyList<BreakSpan> breaks)
    {
        var start = UtcMoments.Of(phase.Steps.Min(step => step.StartedAtUtc)!.Value);
        var end = UtcMoments.Of(phaseEnd);
        var gross = _calculator.CalculatePhaseGross(start, end);
        var breakSeconds = _calculator.CalculatePhaseBreaks(start, end, breaks);
        var net = _calculator.CalculatePhaseNet(gross, breakSeconds);
        return new PhaseTotals(gross, breakSeconds, net, _calculator.IsBelowMinimum(net, phase.MinDurationSecondsSnapshot));
    }

    public async Task CompleteAsync(
        CleaningRecord record,
        RecordPhase phase,
        PhaseTotals totals,
        string? reason,
        IReadOnlyList<BreakSpan> breaks,
        int actorUserId,
        Guid commandId,
        DateTime now,
        CancellationToken cancellationToken)
    {
        phase.Status = totals.BelowMinimum ? PhaseStatus.CompletedBelowMinimum : PhaseStatus.Completed;
        phase.GrossSeconds = totals.GrossSeconds;
        phase.BreakSeconds = totals.BreakSeconds;
        phase.NetSeconds = totals.NetSeconds;
        phase.BelowMinimum = totals.BelowMinimum;
        phase.BelowMinimumReason = totals.BelowMinimum ? reason : null;

        var recordDone = record.Phases.All(item => item.Status is PhaseStatus.Completed or PhaseStatus.CompletedBelowMinimum);
        if (recordDone)
        {
            CompleteRecord(record, breaks, now);
        }

        await _db.SaveChangesAsync(cancellationToken);
        await _events.AppendAsync(
            new EventDraft(
                record.Id,
                EventTypes.PhaseCompleted,
                new { PhaseId = phase.Id, phase.Status, totals.GrossSeconds, totals.BreakSeconds, totals.NetSeconds, totals.BelowMinimum, Reason = phase.BelowMinimumReason },
                actorUserId,
                commandId,
                now),
            cancellationToken);

        if (recordDone)
        {
            await _events.AppendAsync(
                new EventDraft(
                    record.Id,
                    EventTypes.RecordCompleted,
                    new { record.GrossSeconds, record.BreakSeconds, record.NetSeconds, record.EffortSeconds },
                    actorUserId,
                    commandId,
                    now),
                cancellationToken);
        }
    }

    private void CompleteRecord(CleaningRecord record, IReadOnlyList<BreakSpan> breaks, DateTime now)
    {
        var start = UtcMoments.Of(record.StartedAtUtc!.Value);
        var end = UtcMoments.Of(now);
        record.Status = RecordStatus.Completed;
        record.CompletedAtUtc = now;
        record.GrossSeconds = _calculator.CalculateRecordGross(start, end);
        record.BreakSeconds = _calculator.CalculateRecordBreaks(start, end, breaks);
        record.NetSeconds = _calculator.CalculateRecordNet(record.GrossSeconds.Value, record.BreakSeconds.Value);
        record.EffortSeconds = _calculator.CalculateRecordEffort([.. record.Phases.SelectMany(item => item.Steps).Select(step => step.EffortSeconds ?? 0)]);
        record.Version++;
    }
}
