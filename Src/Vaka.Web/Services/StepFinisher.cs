using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

public sealed class StepFinisher : IStepFinisher
{
    private const int ReasonMaxLength = 500;

    private readonly VakaDbContext _db;
    private readonly IWriteTransactionRunner _transactions;
    private readonly IRecordTimeouts _timeouts;
    private readonly IBreakReader _breaks;
    private readonly IPinService _pins;
    private readonly ICleaningEventWriter _events;
    private readonly IProcessedCommands _commands;
    private readonly IDurationCalculator _calculator;
    private readonly IStepWorkCloser _work;
    private readonly IRunCompletion _completion;
    private readonly TimeProvider _time;

    public StepFinisher(
        VakaDbContext db,
        IWriteTransactionRunner transactions,
        IRecordTimeouts timeouts,
        IBreakReader breaks,
        IPinService pins,
        ICleaningEventWriter events,
        IProcessedCommands commands,
        IDurationCalculator calculator,
        IStepWorkCloser work,
        IRunCompletion completion,
        TimeProvider time)
    {
        _db = db;
        _transactions = transactions;
        _timeouts = timeouts;
        _breaks = breaks;
        _pins = pins;
        _events = events;
        _commands = commands;
        _calculator = calculator;
        _work = work;
        _completion = completion;
        _time = time;
    }

    public async Task<FinishOutcome> FinishAsync(int stepId, int actorUserId, FinishStepRequest request, CancellationToken cancellationToken)
    {
        if (request.CommandId == Guid.Empty)
        {
            return new FinishOutcome(CommandResult.Rejected(string.Empty, "İstek kimliği eksik. Sayfayı yenileyip yeniden deneyin."), null);
        }

        if (await _db.GetRecordIdAsync(stepId, cancellationToken) is { } recordId)
        {
            await _timeouts.ApplyAsync(TimeoutScope.ForRecord(recordId), cancellationToken);
        }

        BelowMinimumPrompt? prompt = null;
        var result = await _transactions.RunAsync(async token =>
        {
            var outcome = await FinishInTransactionAsync(stepId, actorUserId, request, token);
            prompt = outcome.Prompt;
            return outcome.Result;
        }, cancellationToken);

        return new FinishOutcome(result, prompt);
    }

    private async Task<FinishOutcome> FinishInTransactionAsync(int stepId, int actorUserId, FinishStepRequest request, CancellationToken cancellationToken)
    {
        if (await _commands.ReplayAsync(request.CommandId, actorUserId, cancellationToken) is { } replay)
        {
            return new FinishOutcome(replay, null);
        }

        if (await _breaks.CheckAsync(actorUserId, cancellationToken) is { } onBreak)
        {
            return new FinishOutcome(onBreak, null);
        }

        var record = await _db.LoadRecordForStepAsync(stepId, cancellationToken);
        if (record is null)
        {
            return new FinishOutcome(CommandResult.NotFound(), null);
        }

        var step = record.OrderedSteps().First(item => item.Id == stepId);
        if (await CheckFinishableAsync(record, step, actorUserId, cancellationToken) is { } failure)
        {
            return new FinishOutcome(failure, null);
        }

        if (await CheckPendingAsync(record, step, cancellationToken) is { } gate)
        {
            return new FinishOutcome(gate, null);
        }

        var now = _time.GetUtcNow().UtcDateTime;
        var breaks = await _db.GetBreakSpansAsync(record.Id, cancellationToken);
        var phase = step.Phase;
        var closesPhase = phase.Steps.All(item => item.Id == step.Id || item.Status == StepStatus.Completed);
        var totals = closesPhase ? _completion.MeasurePhase(phase, now, breaks) : null;
        var reason = request.BelowMinimumReason?.Trim();
        BelowMinimumPrompt? prompt = null;

        if (totals is { BelowMinimum: true })
        {
            prompt = new BelowMinimumPrompt(record.Id, phase.ProcedurePhase.Title, totals.NetSeconds, phase.MinDurationSecondsSnapshot!.Value);
            if (string.IsNullOrEmpty(reason))
            {
                return new FinishOutcome(CommandResult.Rejected("BelowMinimumReason", "Faz süresi minimumun altında. Gerekçe yazın."), prompt);
            }

            if (reason.Length > ReasonMaxLength)
            {
                return new FinishOutcome(CommandResult.Rejected("BelowMinimumReason", $"Gerekçe en çok {ReasonMaxLength} karakter olabilir."), prompt);
            }
        }

        if (await _pins.ConfirmAsync(actorUserId, request.Pin, cancellationToken) is { } pinFailure)
        {
            return new FinishOutcome(pinFailure, prompt);
        }

        await ApplyAsync(record, step, totals, reason, breaks, actorUserId, request.CommandId, now, cancellationToken);
        return new FinishOutcome(CommandResult.Success(record.Id), null);
    }

    private async Task<CommandResult?> CheckFinishableAsync(CleaningRecord record, RecordStep step, int actorUserId, CancellationToken cancellationToken)
    {
        if (record.OwnerUserId != actorUserId && step.Participants.All(item => item.UserId != actorUserId || item.WorkInterval is { EndKind: StepIntervalEndKind.Left }))
        {
            return CommandResult.Forbidden();
        }

        if (record.Status != RecordStatus.InProgress)
        {
            return CommandResult.Rejected(string.Empty, "Kayıt Devam ediyor durumunda değil, adım bitirilemez.");
        }

        if (await _db.RecordBreaks.AnyAsync(item => item.CleaningRecordId == record.Id && item.EndedAtUtc == null, cancellationToken))
        {
            return CommandResult.Rejected(string.Empty, "Kayıtta açık mola var.");
        }

        return step.Status == StepStatus.InProgress
            ? null
            : CommandResult.Rejected(string.Empty, "Adım devam etmiyor, bitirilemez.");
    }

    /// <summary>Kapı (O-11): sahip dışında girişini yapmamış görevli varsa adım bitmez.</summary>
    private async Task<CommandResult?> CheckPendingAsync(CleaningRecord record, RecordStep step, CancellationToken cancellationToken)
    {
        var pendingIds = step.Participants
            .Where(item => item.UserId != record.OwnerUserId && item.WorkInterval is null && !item.IsWorking)
            .Select(item => item.UserId)
            .ToArray();
        if (pendingIds.Length == 0)
        {
            return null;
        }

        var names = await _db.Users.AsNoTracking().Where(user => pendingIds.Contains(user.Id)).OrderBy(user => user.FullName).Select(user => user.FullName).ToListAsync(cancellationToken);
        return CommandResult.Rejected(string.Empty, ParticipantStateRules.PendingMessage(names));
    }

    private async Task ApplyAsync(
        CleaningRecord record,
        RecordStep step,
        PhaseTotals? phaseTotals,
        string? reason,
        List<BreakSpan> breaks,
        int actorUserId,
        Guid commandId,
        DateTime now,
        CancellationToken cancellationToken)
    {
        var start = UtcMoments.Of(step.StartedAtUtc!.Value);
        var end = UtcMoments.Of(now);
        var stepBreaks = _calculator.CalculateStepBreaks(start, end, breaks);
        var duration = _calculator.CalculateStepDuration(start, end, stepBreaks);

        step.Status = StepStatus.Completed;
        step.EndedAtUtc = now;
        step.EndedByUserId = actorUserId;
        step.BreakSeconds = stepBreaks;
        step.DurationSeconds = duration;
        var work = _work.Close(step, StepIntervalEndKind.StepFinished, actorUserId, now, breaks);
        step.EffortSeconds = _calculator.CalculateStepEffort(work.WorkSeconds);

        await _db.SaveChangesAsync(cancellationToken);
        await _events.AppendAsync(
            new EventDraft(
                record.Id,
                EventTypes.StepFinished,
                new { StepId = step.Id, step.DurationSeconds, step.EffortSeconds, step.BreakSeconds, ParticipantUserIds = work.UserIds, ParticipantWorkSeconds = work.WorkSeconds },
                actorUserId,
                commandId,
                now),
            cancellationToken);

        if (phaseTotals is not null)
        {
            await _completion.CompleteAsync(record, step.Phase, phaseTotals, reason, breaks, actorUserId, commandId, now, cancellationToken);
        }

        _commands.Remember(commandId, nameof(FinishAsync), actorUserId, record.Id, now);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
