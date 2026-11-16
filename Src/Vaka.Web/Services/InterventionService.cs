using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

public sealed class InterventionService : IInterventionService
{
    private readonly VakaDbContext _db;
    private readonly IWriteTransactionRunner _transactions;
    private readonly IRecordTimeouts _timeouts;
    private readonly IPinService _pins;
    private readonly ICleaningEventWriter _events;
    private readonly IProcessedCommands _commands;
    private readonly IDurationCalculator _calculator;
    private readonly IStepWorkCloser _work;
    private readonly TimeProvider _time;

    public InterventionService(
        VakaDbContext db,
        IWriteTransactionRunner transactions,
        IRecordTimeouts timeouts,
        IPinService pins,
        ICleaningEventWriter events,
        IProcessedCommands commands,
        IDurationCalculator calculator,
        IStepWorkCloser work,
        TimeProvider time)
    {
        _db = db;
        _transactions = transactions;
        _timeouts = timeouts;
        _pins = pins;
        _events = events;
        _commands = commands;
        _calculator = calculator;
        _work = work;
        _time = time;
    }

    public async Task<CommandResult> InterveneAsync(int recordId, int actorUserId, InterventionRequest request, CancellationToken cancellationToken)
    {
        if (request.CommandId == Guid.Empty)
        {
            return MissingCommandId();
        }

        await _timeouts.ApplyAsync(TimeoutScope.ForRecord(recordId), cancellationToken);
        return await _transactions.RunAsync(async token =>
        {
            if (await _commands.ReplayAsync(request.CommandId, actorUserId, token) is { } replay)
            {
                return replay;
            }

            var record = await _db.CleaningRecords
                .Include(item => item.Phases).ThenInclude(phase => phase.Steps).ThenInclude(step => step.Participants).ThenInclude(participant => participant.WorkInterval)
                .AsSplitQuery()
                .FirstOrDefaultAsync(item => item.Id == recordId, token);
            if (record is null)
            {
                return CommandResult.NotFound();
            }

            var reason = request.Reason?.Trim();
            if (Check(record.Status, request.Type, reason) is { } failure)
            {
                return failure;
            }

            if (await _pins.ConfirmAsync(actorUserId, request.Pin, token) is { } pinFailure)
            {
                return pinFailure;
            }

            await ApplyAsync(record, request.Type!.Value, reason!, actorUserId, request.CommandId, token);
            return CommandResult.Success(recordId);
        }, cancellationToken);
    }

    public async Task<CommandResult> EndBreakAsync(int breakId, int actorUserId, ManagerBreakEndRequest request, CancellationToken cancellationToken)
    {
        if (request.CommandId == Guid.Empty)
        {
            return MissingCommandId();
        }

        return await _transactions.RunAsync(async token =>
        {
            if (await _commands.ReplayAsync(request.CommandId, actorUserId, token) is { } replay)
            {
                return replay;
            }

            var open = await _db.RecordBreaks.FirstOrDefaultAsync(item => item.Id == breakId, token);
            if (open is null)
            {
                return CommandResult.NotFound();
            }

            if (open.EndedAtUtc is not null)
            {
                return CommandResult.Rejected(string.Empty, "Mola zaten bitmiş.");
            }

            var reason = request.Reason?.Trim();
            if (InterventionRules.CheckReason(reason) is { } reasonFailure)
            {
                return reasonFailure;
            }

            if (await _pins.ConfirmAsync(actorUserId, request.Pin, token) is { } pinFailure)
            {
                return pinFailure;
            }

            var now = _time.GetUtcNow().UtcDateTime;
            await CloseBreakAsync(open, BreakEndKind.Manager, reason!, actorUserId, request.CommandId, now, token);
            _commands.Remember(request.CommandId, nameof(EndBreakAsync), actorUserId, open.CleaningRecordId, now);
            await _db.SaveChangesAsync(token);
            return CommandResult.Success(open.CleaningRecordId);
        }, cancellationToken);
    }

    private static CommandResult? Check(RecordStatus status, InterventionType? type, string? reason)
    {
        var allowed = InterventionRules.AllowedTypes(status);
        if (allowed.Length == 0)
        {
            return CommandResult.Rejected(string.Empty, InterventionRules.BlockReason(status)!);
        }

        if (type is not { } chosen || !Enum.IsDefined(chosen))
        {
            return CommandResult.Rejected("Type", "Müdahale türünü seçin.");
        }

        if (!allowed.Contains(chosen))
        {
            return CommandResult.Rejected("Type", "Açıldı durumundaki kayıt yarıda kapatılamaz, yalnız iptal edilir.");
        }

        return InterventionRules.CheckReason(reason);
    }

    private async Task ApplyAsync(CleaningRecord record, InterventionType type, string reason, int actorUserId, Guid commandId, CancellationToken cancellationToken)
    {
        var now = _time.GetUtcNow().UtcDateTime;
        var openBreak = await _db.RecordBreaks.FirstOrDefaultAsync(item => item.CleaningRecordId == record.Id && item.EndedAtUtc == null, cancellationToken);
        if (openBreak is not null)
        {
            await CloseBreakAsync(openBreak, BreakEndKind.Intervention, reason, actorUserId, commandId, now, cancellationToken);
        }

        var steps = record.Phases.SelectMany(phase => phase.Steps).ToList();
        var running = steps.FirstOrDefault(step => step.Status == StepStatus.InProgress);
        if (running is not null)
        {
            await InterruptAsync(record.Id, running, actorUserId, commandId, now, cancellationToken);
        }

        foreach (var phase in record.Phases.Where(phase => phase.Status is not (PhaseStatus.Completed or PhaseStatus.CompletedBelowMinimum)))
        {
            phase.Status = PhaseStatus.Interrupted;
        }

        foreach (var participant in steps.SelectMany(step => step.Participants))
        {
            participant.IsWorking = false;
        }

        record.Status = type == InterventionType.Cancel ? RecordStatus.Cancelled : RecordStatus.Aborted;
        record.ClosedAtUtc = now;
        record.ClosureKind = (int)type;
        record.ClosedByUserId = actorUserId;
        record.ClosureReason = reason;
        record.Version++;
        await _db.SaveChangesAsync(cancellationToken);

        var eventType = type == InterventionType.Cancel ? EventTypes.RecordCancelled : EventTypes.RecordAborted;
        await _events.AppendAsync(
            new EventDraft(record.Id, eventType, new { Reason = reason, ClosureKind = (int)type, InterruptedStepId = running?.Id }, actorUserId, commandId, now),
            cancellationToken);
        _commands.Remember(commandId, nameof(InterveneAsync), actorUserId, record.Id, now);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task InterruptAsync(int recordId, RecordStep step, int actorUserId, Guid commandId, DateTime now, CancellationToken cancellationToken)
    {
        var spans = await _db.GetBreakSpansAsync(recordId, cancellationToken);
        var start = UtcMoments.Of(step.StartedAtUtc!.Value);
        var end = UtcMoments.Of(now);
        var stepBreaks = _calculator.CalculateStepBreaks(start, end, spans);
        var duration = _calculator.CalculateStepDuration(start, end, stepBreaks);

        step.Status = StepStatus.Interrupted;
        step.EndedAtUtc = now;
        step.EndedByUserId = actorUserId;
        step.BreakSeconds = stepBreaks;
        step.DurationSeconds = duration;
        var work = _work.Close(step, StepIntervalEndKind.Interrupted, actorUserId, now, spans);
        step.EffortSeconds = _calculator.CalculateStepEffort(work.WorkSeconds);
        await _db.SaveChangesAsync(cancellationToken);

        await _events.AppendAsync(
            new EventDraft(
                recordId,
                EventTypes.StepInterrupted,
                new { StepId = step.Id, step.DurationSeconds, step.EffortSeconds, step.BreakSeconds, ParticipantUserIds = work.UserIds, ParticipantWorkSeconds = work.WorkSeconds },
                actorUserId,
                commandId,
                now),
            cancellationToken);
    }

    private async Task CloseBreakAsync(RecordBreak item, BreakEndKind kind, string reason, int actorUserId, Guid commandId, DateTime now, CancellationToken cancellationToken)
    {
        item.EndedAtUtc = now;
        item.EndedByUserId = actorUserId;
        item.EndKind = kind;
        item.EndReason = reason;
        await _db.SaveChangesAsync(cancellationToken);

        var seconds = _calculator.CalculateBreakSeconds(UtcMoments.Of(item.StartedAtUtc), UtcMoments.Of(now));
        await _events.AppendAsync(
            new EventDraft(item.CleaningRecordId, EventTypes.BreakEnded, new { BreakId = item.Id, EndKind = kind.ToString(), DurationSeconds = seconds, Reason = reason }, actorUserId, commandId, now),
            cancellationToken);
    }

    private static CommandResult MissingCommandId() =>
        CommandResult.Rejected(string.Empty, "İstek kimliği eksik. Sayfayı yenileyip yeniden deneyin.");
}
