using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

public sealed class RecordOpener : IRecordOpener
{
    private const string CommandType = "OpenRecord";

    private readonly VakaDbContext _db;
    private readonly IWriteTransactionRunner _transactions;
    private readonly IRecordTimeouts _timeouts;
    private readonly IRecordOpenValidator _validator;
    private readonly IBreakReader _breaks;
    private readonly IRecordBlockFinder _blocks;
    private readonly IRecordPhaseBuilder _phases;
    private readonly IPinService _pins;
    private readonly IShiftLocator _shifts;
    private readonly IDurationCalculator _calculator;
    private readonly INumberSequences _numbers;
    private readonly ICleaningEventWriter _events;
    private readonly IProcessedCommands _commands;
    private readonly ITurkeyClock _clock;
    private readonly TimeProvider _time;

    public RecordOpener(
        VakaDbContext db,
        IWriteTransactionRunner transactions,
        IRecordTimeouts timeouts,
        IRecordOpenValidator validator,
        IBreakReader breaks,
        IRecordBlockFinder blocks,
        IRecordPhaseBuilder phases,
        IPinService pins,
        IShiftLocator shifts,
        IDurationCalculator calculator,
        INumberSequences numbers,
        ICleaningEventWriter events,
        IProcessedCommands commands,
        ITurkeyClock clock,
        TimeProvider time)
    {
        _db = db;
        _transactions = transactions;
        _timeouts = timeouts;
        _validator = validator;
        _breaks = breaks;
        _blocks = blocks;
        _phases = phases;
        _pins = pins;
        _shifts = shifts;
        _calculator = calculator;
        _numbers = numbers;
        _events = events;
        _commands = commands;
        _clock = clock;
        _time = time;
    }

    public async Task<OpenOutcome> OpenAsync(int ownerUserId, RecordOpenRequest request, CancellationToken cancellationToken)
    {
        if (request.CommandId == Guid.Empty)
        {
            return new OpenOutcome(CommandResult.Rejected(string.Empty, "İstek kimliği eksik. Sayfayı yenileyip yeniden deneyin."), null);
        }

        await _timeouts.ApplyAsync(TimeoutScope.ForMachineAndOwner(request.MachineId ?? 0, ownerUserId), cancellationToken);

        int? blockingId = null;
        var result = await _transactions.RunAsync(async token =>
        {
            var outcome = await OpenInTransactionAsync(ownerUserId, request, token);
            blockingId = outcome.BlockingRecordId;
            return outcome.Result;
        }, cancellationToken);

        return new OpenOutcome(result, blockingId);
    }

    private async Task<OpenOutcome> OpenInTransactionAsync(int ownerUserId, RecordOpenRequest request, CancellationToken cancellationToken)
    {
        var replay = await _commands.ReplayAsync(request.CommandId, ownerUserId, cancellationToken);
        if (replay is not null)
        {
            return new OpenOutcome(replay, null);
        }

        if (await _breaks.CheckAsync(ownerUserId, cancellationToken) is { } onBreak)
        {
            return new OpenOutcome(onBreak, null);
        }

        if (!await _db.Users.AnyAsync(user => user.Id == ownerUserId && user.IsActive, cancellationToken))
        {
            return new OpenOutcome(CommandResult.Forbidden(), null);
        }

        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (validation.Plan is not { } plan)
        {
            return new OpenOutcome(CommandResult.Rejected(validation.Errors), null);
        }

        var blocked = await _blocks.FindAsync(plan.Machine.Id, ownerUserId, cancellationToken);
        if (blocked is not null)
        {
            return blocked;
        }

        var pinFailure = await _pins.ConfirmAsync(ownerUserId, request.Pin, cancellationToken);
        if (pinFailure is not null)
        {
            return new OpenOutcome(pinFailure, null);
        }

        return await CreateAsync(ownerUserId, request.CommandId, plan, cancellationToken);
    }

    private async Task<OpenOutcome> CreateAsync(int ownerUserId, Guid commandId, RecordOpenPlan plan, CancellationToken cancellationToken)
    {
        var now = _time.GetUtcNow();
        var machine = plan.Machine;
        var shift = await _shifts.FindAtAsync(machine.FacilityId, now, cancellationToken);
        var lockExpiresAt = _calculator.CalculateLockExpiry(now, plan.LockMinutes, shift);
        var year = _clock.Today().Year;
        var sequence = await _numbers.NextAsync(RecordNumbers.RecordScope(machine.Id), year, cancellationToken);
        var workOrderNo = plan.WorkOrderNo.Length > 0
            ? plan.WorkOrderNo
            : RecordNumbers.FormatWorkOrder(machine.FacilityCode, year, await _numbers.NextAsync(RecordNumbers.WorkOrderScope(machine.FacilityId), year, cancellationToken));

        var record = new CleaningRecord
        {
            RecordNo = RecordNumbers.Format(machine.FacilityCode, machine.LineCode, machine.Code, CleaningTypeKind.Planned, year, sequence),
            Kind = CleaningTypeKind.Planned,
            Status = RecordStatus.Opened,
            ProcedureVersionId = plan.VersionId,
            OwnerUserId = ownerUserId,
            WorkOrderNo = workOrderNo,
            Note = string.IsNullOrEmpty(plan.Note) ? null : plan.Note,
            LockMinutes = plan.LockMinutes,
            LockExpiresAtUtc = lockExpiresAt.UtcDateTime,
            OpenedAtUtc = now.UtcDateTime
        };
        record.ApplyMachine(machine);

        foreach (var phase in await _phases.BuildAsync(plan.VersionId, ownerUserId, cancellationToken))
        {
            record.Phases.Add(phase);
        }

        if (record.Phases.Count == 0)
        {
            return new OpenOutcome(CommandResult.Rejected(string.Empty, "Temizlik tipinin yürürlükteki versiyonunda faz yok. Yöneticinize bildirin."), null);
        }

        foreach (var lot in plan.Lots)
        {
            record.Materials.Add(lot.ToRecordMaterial(ownerUserId, now.UtcDateTime));
        }

        _db.CleaningRecords.Add(record);
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (ConstraintViolations.IsRecordOpenConflict(exception))
        {
            _db.ChangeTracker.Clear();
            var blocked = await _blocks.FindAsync(machine.Id, ownerUserId, cancellationToken);
            return blocked ?? new OpenOutcome(CommandResult.Rejected(string.Empty, "Kayıt şu an açılamadı. Birkaç saniye sonra yeniden deneyin."), null);
        }

        await _events.AppendAsync(
            new EventDraft(
                record.Id,
                EventTypes.RecordOpened,
                new
                {
                    record.RecordNo,
                    MachineId = machine.Id,
                    plan.CleaningTypeId,
                    ProcedureVersionId = plan.VersionId,
                    plan.VersionNo,
                    plan.LockMinutes,
                    LockExpiresAtUtc = record.LockExpiresAtUtc,
                    WorkOrderNo = workOrderNo,
                    MaterialLotIds = plan.Lots.Select(lot => lot.LotId),
                    MaterialLots = plan.Lots.Select(lot => new { lot.LotId, lot.LotNo, ExpiredAtEntry = lot.IsExpired, lot.ExpiredReason })
                },
                ownerUserId,
                commandId,
                now.UtcDateTime),
            cancellationToken);

        _commands.Remember(commandId, CommandType, ownerUserId, record.Id, now.UtcDateTime);
        await _db.SaveChangesAsync(cancellationToken);
        return new OpenOutcome(CommandResult.Success(record.Id), null);
    }
}
