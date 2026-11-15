using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

public sealed class EmergencyOpener : IEmergencyOpener
{
    private const string CommandType = "OpenEmergency";

    private readonly VakaDbContext _db;
    private readonly IWriteTransactionRunner _transactions;
    private readonly IRecordTimeouts _timeouts;
    private readonly IEmergencyOpenValidator _validator;
    private readonly IBreakReader _breaks;
    private readonly IRecordBlockFinder _blocks;
    private readonly IRecordPhaseBuilder _phases;
    private readonly IStepStartApplier _applier;
    private readonly IPinService _pins;
    private readonly INumberSequences _numbers;
    private readonly ICleaningEventWriter _events;
    private readonly IProcessedCommands _commands;
    private readonly ITurkeyClock _clock;
    private readonly TimeProvider _time;

    public EmergencyOpener(
        VakaDbContext db,
        IWriteTransactionRunner transactions,
        IRecordTimeouts timeouts,
        IEmergencyOpenValidator validator,
        IBreakReader breaks,
        IRecordBlockFinder blocks,
        IRecordPhaseBuilder phases,
        IStepStartApplier applier,
        IPinService pins,
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
        _applier = applier;
        _pins = pins;
        _numbers = numbers;
        _events = events;
        _commands = commands;
        _clock = clock;
        _time = time;
    }

    public async Task<OpenOutcome> OpenAsync(int ownerUserId, EmergencyOpenRequest request, CancellationToken cancellationToken)
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

    private async Task<OpenOutcome> OpenInTransactionAsync(int ownerUserId, EmergencyOpenRequest request, CancellationToken cancellationToken)
    {
        if (await _commands.ReplayAsync(request.CommandId, ownerUserId, cancellationToken) is { } replay)
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

        if (await _blocks.FindAsync(plan.Machine.Id, ownerUserId, cancellationToken) is { } blocked)
        {
            return blocked;
        }

        if (await _pins.ConfirmAsync(ownerUserId, request.Pin, cancellationToken) is { } pinFailure)
        {
            return new OpenOutcome(pinFailure, null);
        }

        return await CreateAsync(ownerUserId, request.CommandId, plan, cancellationToken);
    }

    private async Task<OpenOutcome> CreateAsync(int ownerUserId, Guid commandId, EmergencyPlan plan, CancellationToken cancellationToken)
    {
        var now = _time.GetUtcNow().UtcDateTime;
        var machine = plan.Machine;
        var year = _clock.Today().Year;
        var sequence = await _numbers.NextAsync(RecordNumbers.RecordScope(machine.Id), year, cancellationToken);

        var record = new CleaningRecord
        {
            RecordNo = RecordNumbers.Format(machine.FacilityCode, machine.LineCode, machine.Code, CleaningTypeKind.Emergency, year, sequence),
            Kind = CleaningTypeKind.Emergency,
            Status = RecordStatus.Opened,
            ProcedureVersionId = plan.VersionId,
            OwnerUserId = ownerUserId,
            EmergencyDescription = plan.Description,
            OpenedAtUtc = now
        };
        record.ApplyMachine(machine);

        foreach (var phase in await _phases.BuildAsync(plan.VersionId, ownerUserId, cancellationToken))
        {
            record.Phases.Add(phase);
        }

        if (record.Phases.All(phase => phase.Steps.Count == 0))
        {
            return new OpenOutcome(CommandResult.Rejected(string.Empty, "Acil tipin yürürlükteki versiyonunda adım yok. Yöneticinize bildirin."), null);
        }

        foreach (var lot in plan.Lots)
        {
            record.Materials.Add(lot.ToRecordMaterial(ownerUserId, now));
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

        var opened = new
        {
            record.RecordNo,
            Kind = nameof(CleaningTypeKind.Emergency),
            MachineId = machine.Id,
            plan.CleaningTypeId,
            ProcedureVersionId = plan.VersionId,
            plan.VersionNo,
            plan.Description,
            MaterialLotIds = plan.Lots.Select(lot => lot.LotId),
            MaterialLots = plan.Lots.Select(lot => new { lot.LotId, lot.LotNo, ExpiredAtEntry = lot.IsExpired, lot.ExpiredReason })
        };
        await _events.AppendAsync(new EventDraft(record.Id, EventTypes.RecordOpened, opened, ownerUserId, commandId, now), cancellationToken);

        var firstStepId = record.Phases.SelectMany(phase => phase.Steps).First().Id;
        _db.ChangeTracker.Clear();
        var loaded = await _db.LoadRecordForStepAsync(firstStepId, cancellationToken)
            ?? throw new InvalidOperationException("Yeni açılan acil kayıt işlem içinde bulunamadı.");
        var first = loaded.OrderedSteps()[0];
        var failure = await _applier.CheckParticipantsAsync(first, cancellationToken)
            ?? await _applier.ApplyAsync(loaded, first, true, ownerUserId, commandId, now, cancellationToken);
        if (failure is not null)
        {
            return new OpenOutcome(failure, null);
        }

        _commands.Remember(commandId, CommandType, ownerUserId, record.Id, now);
        await _db.SaveChangesAsync(cancellationToken);
        return new OpenOutcome(CommandResult.Success(record.Id), null);
    }
}
