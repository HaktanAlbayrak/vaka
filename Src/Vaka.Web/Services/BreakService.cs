using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

public sealed class BreakService : IBreakService
{
    private readonly VakaDbContext _db;
    private readonly IWriteTransactionRunner _transactions;
    private readonly IRecordTimeouts _timeouts;
    private readonly IBreakReader _breaks;
    private readonly IPinService _pins;
    private readonly ICleaningEventWriter _events;
    private readonly IProcessedCommands _commands;
    private readonly IDurationCalculator _calculator;
    private readonly TimeProvider _time;

    public BreakService(
        VakaDbContext db,
        IWriteTransactionRunner transactions,
        IRecordTimeouts timeouts,
        IBreakReader breaks,
        IPinService pins,
        ICleaningEventWriter events,
        IProcessedCommands commands,
        IDurationCalculator calculator,
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
        _time = time;
    }

    public async Task<CommandResult> StartAsync(int recordId, int actorUserId, StartBreakRequest request, CancellationToken cancellationToken)
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

            if (await _breaks.CheckAsync(actorUserId, token) is { } onBreak)
            {
                return onBreak;
            }

            if (await CheckStartableAsync(recordId, actorUserId, token) is { } failure)
            {
                return failure;
            }

            if (await _pins.ConfirmAsync(actorUserId, request.Pin, token) is { } pinFailure)
            {
                return pinFailure;
            }

            var now = _time.GetUtcNow().UtcDateTime;
            var started = new RecordBreak { CleaningRecordId = recordId, StartedByUserId = actorUserId, StartedAtUtc = now };
            _db.RecordBreaks.Add(started);
            try
            {
                await _db.SaveChangesAsync(token);
            }
            catch (DbUpdateException exception) when (ConstraintViolations.IsUniqueViolation(exception, "RecordBreaks."))
            {
                _db.ChangeTracker.Clear();
                var reason = ConstraintViolations.IsUniqueViolation(exception, "RecordBreaks.StartedByUserId")
                    ? "Zaten açık molanız var."
                    : "Kayıtta açık mola var.";
                return CommandResult.Rejected(string.Empty, reason);
            }

            await _events.AppendAsync(
                new EventDraft(recordId, EventTypes.BreakStarted, new { BreakId = started.Id, DuringStepId = (int?)null }, actorUserId, request.CommandId, now),
                token);
            _commands.Remember(request.CommandId, nameof(StartAsync), actorUserId, recordId, now);
            await _db.SaveChangesAsync(token);
            return CommandResult.Success(recordId);
        }, cancellationToken);
    }

    public async Task<CommandResult> EndAsync(int breakId, int actorUserId, EndBreakRequest request, CancellationToken cancellationToken)
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

            if (open.StartedByUserId != actorUserId)
            {
                return CommandResult.Forbidden();
            }

            if (open.EndedAtUtc is not null)
            {
                return CommandResult.Rejected(string.Empty, "Mola zaten bitmiş.");
            }

            if (await _pins.ConfirmAsync(actorUserId, request.Pin, token) is { } pinFailure)
            {
                return pinFailure;
            }

            var now = _time.GetUtcNow().UtcDateTime;
            open.EndedAtUtc = now;
            open.EndedByUserId = actorUserId;
            open.EndKind = BreakEndKind.Person;
            await _db.SaveChangesAsync(token);

            var seconds = _calculator.CalculateBreakSeconds(UtcMoments.Of(open.StartedAtUtc), UtcMoments.Of(now));
            await _events.AppendAsync(
                new EventDraft(open.CleaningRecordId, EventTypes.BreakEnded, new { BreakId = open.Id, EndKind = BreakEndKind.Person.ToString(), DurationSeconds = seconds }, actorUserId, request.CommandId, now),
                token);
            _commands.Remember(request.CommandId, nameof(EndAsync), actorUserId, open.CleaningRecordId, now);
            await _db.SaveChangesAsync(token);
            return CommandResult.Success(open.CleaningRecordId);
        }, cancellationToken);
    }

    private async Task<CommandResult?> CheckStartableAsync(int recordId, int actorUserId, CancellationToken cancellationToken)
    {
        var record = await _db.CleaningRecords
            .AsNoTracking()
            .Where(item => item.Id == recordId)
            .Select(item => new { item.Status, item.OwnerUserId })
            .FirstOrDefaultAsync(cancellationToken);
        if (record is null)
        {
            return CommandResult.NotFound();
        }

        var steps = await _db.RecordSteps
            .AsNoTracking()
            .Where(step => step.Phase.CleaningRecordId == recordId)
            .OrderBy(step => step.Phase.ProcedurePhase.OrderNo).ThenBy(step => step.ProcedureStep.OrderNo)
            .Select(step => new { step.Status, Participants = step.Participants.Select(item => item.UserId).ToList() })
            .ToListAsync(cancellationToken);
        var next = steps.FirstOrDefault(step => step.Status != StepStatus.Completed);
        var isMember = record.OwnerUserId == actorUserId || (next?.Participants.Contains(actorUserId) ?? false);
        if (!isMember)
        {
            return CommandResult.Forbidden();
        }

        if (record.Status == RecordStatus.Opened)
        {
            return CommandResult.Rejected(string.Empty, "Mola ilk adım başladıktan sonra, iki adım arasında verilir.");
        }

        if (record.Status != RecordStatus.InProgress)
        {
            return CommandResult.Rejected(string.Empty, record.Status == RecordStatus.TimedOut
                ? "Kilit süresi doldu, kayıt Zaman aşımına düştü."
                : "Kayıt bu durumda mola verilemez.");
        }

        if (steps.Any(step => step.Status == StepStatus.InProgress))
        {
            return CommandResult.Rejected(string.Empty, "Mola iki adım arasında verilir.");
        }

        if (await _db.RecordBreaks.AnyAsync(item => item.CleaningRecordId == recordId && item.EndedAtUtc == null, cancellationToken))
        {
            return CommandResult.Rejected(string.Empty, "Kayıtta açık mola var.");
        }

        var elsewhere = await _db.StepParticipants
            .AsNoTracking()
            .Where(item => item.UserId == actorUserId && item.IsWorking)
            .Select(item => item.Step.Phase.Record.RecordNo)
            .FirstOrDefaultAsync(cancellationToken);
        return elsewhere is null
            ? null
            : CommandResult.Rejected(string.Empty, $"Başka kayıtta devam eden adımdasınız: {elsewhere}. Mola verilemez.");
    }

    private static CommandResult MissingCommandId() =>
        CommandResult.Rejected(string.Empty, "İstek kimliği eksik. Sayfayı yenileyip yeniden deneyin.");
}
