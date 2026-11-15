using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

public sealed class RecordMaterialService : IRecordMaterialService
{
    private const int ReasonMaxLength = 500;

    private readonly VakaDbContext _db;
    private readonly IWriteTransactionRunner _transactions;
    private readonly IRecordTimeouts _timeouts;
    private readonly IPinService _pins;
    private readonly ICleaningEventWriter _events;
    private readonly IProcessedCommands _commands;
    private readonly IDurationCalculator _calculator;
    private readonly ITurkeyClock _clock;
    private readonly TimeProvider _time;

    public RecordMaterialService(
        VakaDbContext db,
        IWriteTransactionRunner transactions,
        IRecordTimeouts timeouts,
        IPinService pins,
        ICleaningEventWriter events,
        IProcessedCommands commands,
        IDurationCalculator calculator,
        ITurkeyClock clock,
        TimeProvider time)
    {
        _db = db;
        _transactions = transactions;
        _timeouts = timeouts;
        _pins = pins;
        _events = events;
        _commands = commands;
        _calculator = calculator;
        _clock = clock;
        _time = time;
    }

    public async Task<CommandResult> AddAsync(int recordId, int actorUserId, AddMaterialRequest request, CancellationToken cancellationToken)
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

            if (await CheckEditableAsync(recordId, actorUserId, token) is { } failure)
            {
                return failure;
            }

            var lot = await _db.MaterialLots
                .AsNoTracking()
                .Where(item => item.Id == request.MaterialLotId && item.Material.IsActive)
                .Select(item => new { item.Id, Code = item.Material.Code, Name = item.Material.Name, item.LotNo, item.ExpiryDate })
                .FirstOrDefaultAsync(token);

            if (lot is null)
            {
                return CommandResult.Rejected("MaterialLotId", "Lot seçilemez.");
            }

            if (await _db.RecordMaterials.AnyAsync(item => item.CleaningRecordId == recordId && item.MaterialLotId == lot.Id && item.RemovedAtUtc == null, token))
            {
                return CommandResult.Rejected("MaterialLotId", "Bu lot kayda zaten eklenmiş.");
            }

            var expired = _calculator.IsLotExpired(lot.ExpiryDate, _clock.Today());
            var reason = request.ExpiredReason?.Trim();
            if (expired && string.IsNullOrEmpty(reason))
            {
                return CommandResult.Rejected("ExpiredReason", "SKT geçmiş lot için gerekçe girin.");
            }

            if (reason?.Length > ReasonMaxLength)
            {
                return CommandResult.Rejected("ExpiredReason", $"Gerekçe en çok {ReasonMaxLength} karakter olabilir.");
            }

            if (await _pins.ConfirmAsync(actorUserId, request.Pin, token) is { } pinFailure)
            {
                return pinFailure;
            }

            var now = _time.GetUtcNow().UtcDateTime;
            _db.RecordMaterials.Add(new RecordMaterial
            {
                CleaningRecordId = recordId,
                MaterialLotId = lot.Id,
                MaterialCodeSnapshot = lot.Code,
                MaterialNameSnapshot = lot.Name,
                LotNoSnapshot = lot.LotNo,
                ExpiryDateSnapshot = lot.ExpiryDate,
                ExpiredAtEntry = expired,
                ExpiredReason = expired ? reason : null,
                EnteredByUserId = actorUserId,
                EnteredAtUtc = now
            });
            await _db.SaveChangesAsync(token);

            await _events.AppendAsync(
                new EventDraft(recordId, EventTypes.MaterialAdded, new { MaterialLotId = lot.Id, lot.LotNo, ExpiredAtEntry = expired, ExpiredReason = expired ? reason : null }, actorUserId, request.CommandId, now),
                token);
            _commands.Remember(request.CommandId, nameof(AddAsync), actorUserId, recordId, now);
            await _db.SaveChangesAsync(token);
            return CommandResult.Success(recordId);
        }, cancellationToken);
    }

    public async Task<CommandResult> RemoveAsync(int recordId, int actorUserId, RemoveMaterialRequest request, CancellationToken cancellationToken)
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

            if (await CheckEditableAsync(recordId, actorUserId, token) is { } failure)
            {
                return failure;
            }

            var material = await _db.RecordMaterials
                .FirstOrDefaultAsync(item => item.Id == request.RecordMaterialId && item.CleaningRecordId == recordId && item.RemovedAtUtc == null, token);
            if (material is null)
            {
                return CommandResult.Rejected(string.Empty, "Malzeme satırı bulunamadı ya da zaten çıkarılmış.");
            }

            if (await _pins.ConfirmAsync(actorUserId, request.Pin, token) is { } pinFailure)
            {
                return pinFailure;
            }

            var now = _time.GetUtcNow().UtcDateTime;
            material.RemovedAtUtc = now;
            await _db.SaveChangesAsync(token);

            await _events.AppendAsync(
                new EventDraft(recordId, EventTypes.MaterialRemoved, new { material.MaterialLotId, LotNo = material.LotNoSnapshot }, actorUserId, request.CommandId, now),
                token);
            _commands.Remember(request.CommandId, nameof(RemoveAsync), actorUserId, recordId, now);
            await _db.SaveChangesAsync(token);
            return CommandResult.Success(recordId);
        }, cancellationToken);
    }

    private async Task<CommandResult?> CheckEditableAsync(int recordId, int actorUserId, CancellationToken cancellationToken)
    {
        var record = await _db.CleaningRecords
            .AsNoTracking()
            .Where(item => item.Id == recordId)
            .Select(item => new
            {
                item.OwnerUserId,
                item.Status,
                StepStarted = item.Phases.Any(phase => phase.Steps.Any(step => step.StartedAtUtc != null))
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (record is null)
        {
            return CommandResult.NotFound();
        }

        if (record.OwnerUserId != actorUserId)
        {
            return CommandResult.Forbidden();
        }

        if (record.Status == RecordStatus.TimedOut)
        {
            return CommandResult.Rejected(string.Empty, "Kilit süresi doldu, kayıt Zaman aşımına düştü. Malzeme değiştirilemez.");
        }

        return record.Status != RecordStatus.Opened || record.StepStarted
            ? CommandResult.Rejected(string.Empty, "Malzeme yalnız ilk adım başlamadan önce eklenir ya da çıkarılır.")
            : null;
    }

    private static CommandResult MissingCommandId() =>
        CommandResult.Rejected(string.Empty, "İstek kimliği eksik. Sayfayı yenileyip yeniden deneyin.");
}
