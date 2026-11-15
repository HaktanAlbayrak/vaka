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

        var blocked = await FindBlockAsync(plan.Machine.Id, ownerUserId, cancellationToken);
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

    /// <summary>Makine kilidi (BR-23) ve sahip kilidi (BR-37) için okuma denetimi; eşzamanlı yarışta asıl garanti veritabanı index'leridir.</summary>
    private async Task<OpenOutcome?> FindBlockAsync(int machineId, int ownerUserId, CancellationToken cancellationToken)
    {
        var onMachine = await _db.CleaningRecords
            .AsNoTracking()
            .Where(record => record.MachineId == machineId
                && (record.Status == RecordStatus.Opened || record.Status == RecordStatus.InProgress || record.Status == RecordStatus.OnHold))
            .Select(record => new { record.Id, OwnerName = record.Owner.FullName, record.Status, record.OpenedAtUtc })
            .FirstOrDefaultAsync(cancellationToken);

        if (onMachine is not null)
        {
            var summary = RecordLabels.ActiveRecordSummary(onMachine.OwnerName, onMachine.Status, _clock.ToLocal(onMachine.OpenedAtUtc));
            return new OpenOutcome(CommandResult.Rejected(string.Empty, "Makinede açık kayıt var. " + summary), onMachine.Id);
        }

        var own = await _db.CleaningRecords
            .AsNoTracking()
            .Where(record => record.OwnerUserId == ownerUserId
                && (record.Status == RecordStatus.Opened || record.Status == RecordStatus.InProgress))
            .Select(record => new { record.Id, record.RecordNo, record.Status })
            .FirstOrDefaultAsync(cancellationToken);

        return own is null
            ? null
            : new OpenOutcome(
                CommandResult.Rejected(string.Empty, $"Açık kaydınız var: {own.RecordNo} ({RecordLabels.Status(own.Status).ToLowerInvariant()}). Yeni kayıt açmadan önce onu sürdürün."),
                own.Id);
    }

    private async Task<OpenOutcome> CreateAsync(int ownerUserId, Guid commandId, RecordOpenPlan plan, CancellationToken cancellationToken)
    {
        var now = _time.GetUtcNow();
        var machine = plan.Machine;
        var shift = await _shifts.FindAtAsync(machine.FacilityId, now, cancellationToken);
        var lockExpiresAt = _calculator.CalculateLockExpiry(now, plan.LockMinutes, shift);
        var year = _clock.Today().Year;
        var sequence = await _numbers.NextAsync(RecordNumbers.RecordScope(machine.Id), year, cancellationToken);

        var record = new CleaningRecord
        {
            RecordNo = RecordNumbers.Format(machine.FacilityCode, machine.LineCode, machine.Code, CleaningTypeKind.Planned, year, sequence),
            Kind = CleaningTypeKind.Planned,
            Status = RecordStatus.Opened,
            MachineId = machine.Id,
            ProcedureVersionId = plan.VersionId,
            OwnerUserId = ownerUserId,
            FacilityCodeSnapshot = machine.FacilityCode,
            FacilityNameSnapshot = machine.FacilityName,
            LineCodeSnapshot = machine.LineCode,
            LineNameSnapshot = machine.LineName,
            MachineCodeSnapshot = machine.Code,
            MachineNameSnapshot = machine.Name,
            WorkOrderNo = plan.WorkOrderNo,
            Note = string.IsNullOrEmpty(plan.Note) ? null : plan.Note,
            LockMinutes = plan.LockMinutes,
            LockExpiresAtUtc = lockExpiresAt.UtcDateTime,
            OpenedAtUtc = now.UtcDateTime
        };

        await AddPhasesAsync(record, ownerUserId, cancellationToken);
        if (record.Phases.Count == 0)
        {
            return new OpenOutcome(CommandResult.Rejected(string.Empty, "Temizlik tipinin yürürlükteki versiyonunda faz yok. Yöneticinize bildirin."), null);
        }

        foreach (var lot in plan.Lots)
        {
            record.Materials.Add(new RecordMaterial
            {
                MaterialLotId = lot.LotId,
                MaterialCodeSnapshot = lot.MaterialCode,
                MaterialNameSnapshot = lot.MaterialName,
                LotNoSnapshot = lot.LotNo,
                ExpiryDateSnapshot = lot.ExpiryDate,
                ExpiredAtEntry = lot.IsExpired,
                ExpiredReason = lot.ExpiredReason,
                EnteredByUserId = ownerUserId,
                EnteredAtUtc = now.UtcDateTime
            });
        }

        _db.CleaningRecords.Add(record);
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (ConstraintViolations.IsRecordOpenConflict(exception))
        {
            _db.ChangeTracker.Clear();
            var blocked = await FindBlockAsync(machine.Id, ownerUserId, cancellationToken);
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
                    plan.WorkOrderNo,
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

    private async Task AddPhasesAsync(CleaningRecord record, int ownerUserId, CancellationToken cancellationToken)
    {
        var phases = await _db.ProcedurePhases
            .AsNoTracking()
            .Where(phase => phase.ProcedureVersionId == record.ProcedureVersionId)
            .OrderBy(phase => phase.OrderNo)
            .Select(phase => new
            {
                phase.Id,
                phase.MinDurationSeconds,
                StepIds = phase.Steps.OrderBy(step => step.OrderNo).Select(step => step.Id).ToList()
            })
            .ToListAsync(cancellationToken);

        foreach (var phase in phases)
        {
            record.Phases.Add(new RecordPhase
            {
                ProcedurePhaseId = phase.Id,
                Status = PhaseStatus.Waiting,
                MinDurationSecondsSnapshot = phase.MinDurationSeconds,
                Steps = phase.StepIds.Select(stepId => new RecordStep
                {
                    ProcedureStepId = stepId,
                    Status = StepStatus.Waiting,
                    Participants = [new StepParticipant { UserId = ownerUserId, AddedByUserId = ownerUserId }]
                }).ToList()
            });
        }
    }
}
