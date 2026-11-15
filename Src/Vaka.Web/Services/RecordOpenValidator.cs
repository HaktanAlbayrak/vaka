using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;

namespace Vaka.Web.Services;

public sealed class RecordOpenValidator : IRecordOpenValidator
{
    private const int WorkOrderMaxLength = 64;
    private const int NoteMaxLength = 1000;
    private const int ReasonMaxLength = 500;

    private readonly VakaDbContext _db;
    private readonly IDurationCalculator _calculator;
    private readonly ISystemSettingsReader _settings;
    private readonly ITurkeyClock _clock;

    public RecordOpenValidator(VakaDbContext db, IDurationCalculator calculator, ISystemSettingsReader settings, ITurkeyClock clock)
    {
        _db = db;
        _calculator = calculator;
        _settings = settings;
        _clock = clock;
    }

    public async Task<OpenValidation> ValidateAsync(RecordOpenRequest request, CancellationToken cancellationToken)
    {
        var errors = new List<FieldError>();
        var machine = await CheckMachineAsync(request, errors, cancellationToken);
        var type = machine is null ? null : await CheckTypeAsync(request, machine.Id, errors, cancellationToken);

        var workOrder = request.WorkOrderNo?.Trim() ?? string.Empty;
        if (workOrder.Length == 0)
        {
            errors.Add(new FieldError("WorkOrderNo", "İş emri numarasını girin."));
        }
        else if (workOrder.Length > WorkOrderMaxLength)
        {
            errors.Add(new FieldError("WorkOrderNo", $"İş emri numarası en çok {WorkOrderMaxLength} karakter olabilir."));
        }

        var note = request.Note?.Trim();
        if (note?.Length > NoteMaxLength)
        {
            errors.Add(new FieldError("Note", $"Açıklama en çok {NoteMaxLength} karakter olabilir."));
        }

        var maxLock = await _settings.GetValueAsync(SettingKeys.MaxLockMinutes, cancellationToken);
        if (!_calculator.IsLockWithinLimit(request.LockMinutes, maxLock))
        {
            errors.Add(new FieldError("LockMinutes", $"Kilit süresi 1 ile {maxLock} dakika arasında olmalı."));
        }

        var lots = await CheckLotsAsync(request, errors, cancellationToken);
        if (errors.Count > 0 || machine is null || type is null)
        {
            return new OpenValidation(null, errors);
        }

        return new OpenValidation(
            new RecordOpenPlan(machine, type.CleaningTypeId, type.VersionId, type.VersionNo, workOrder, note, request.LockMinutes, lots),
            errors);
    }

    private async Task<PlannedMachine?> CheckMachineAsync(RecordOpenRequest request, List<FieldError> errors, CancellationToken cancellationToken)
    {
        var machine = request.MachineId is null
            ? null
            : await _db.Machines
                .AsNoTracking()
                .Where(item => item.Id == request.MachineId)
                .Select(item => new
                {
                    Row = new PlannedMachine(
                        item.Id, item.Code, item.Name, item.Line.Id, item.Line.Code, item.Line.Name,
                        item.Line.FacilityId, item.Line.Facility.Code, item.Line.Facility.Name),
                    item.IsActive,
                    item.RetiredAtUtc,
                    LineActive = item.Line.IsActive,
                    FacilityActive = item.Line.Facility.IsActive
                })
                .FirstOrDefaultAsync(cancellationToken);

        if (machine is null)
        {
            errors.Add(new FieldError("MachineId", "Makine seçin."));
            return null;
        }

        var before = errors.Count;
        if (request.FacilityId != machine.Row.FacilityId || !machine.FacilityActive)
        {
            errors.Add(new FieldError("FacilityId", "Tesis seçilen makineye uygun değil ya da aktif değil."));
        }

        if (request.LineId != machine.Row.LineId || !machine.LineActive)
        {
            errors.Add(new FieldError("LineId", "Hat seçilen makineye uygun değil ya da aktif değil."));
        }

        if (!machine.IsActive || machine.RetiredAtUtc is not null)
        {
            errors.Add(new FieldError("MachineId", "Makine kullanımda değil."));
        }

        return errors.Count == before ? machine.Row : null;
    }

    private async Task<ChosenType?> CheckTypeAsync(RecordOpenRequest request, int machineId, List<FieldError> errors, CancellationToken cancellationToken)
    {
        var type = request.CleaningTypeId is null
            ? null
            : await _db.ProcedureVersions
                .AsNoTracking()
                .Where(version => version.CleaningTypeId == request.CleaningTypeId
                    && version.Status == ProcedureVersionStatus.Current
                    && version.CleaningType.IsActive
                    && version.CleaningType.Kind == CleaningTypeKind.Planned
                    && version.CleaningType.MachineLinks.Any(link => link.MachineId == machineId))
                .Select(version => new ChosenType(version.CleaningTypeId, version.Id, version.VersionNo))
                .FirstOrDefaultAsync(cancellationToken);

        if (type is null)
        {
            errors.Add(new FieldError("CleaningTypeId", "Temizlik tipi seçilen makineye eşli değil ya da yürürlükte versiyonu yok."));
        }

        return type;
    }

    private async Task<IReadOnlyList<PlannedLot>> CheckLotsAsync(RecordOpenRequest request, List<FieldError> errors, CancellationToken cancellationToken)
    {
        var ids = request.Materials.Select(item => item.MaterialLotId).Distinct().ToArray();
        var found = await _db.MaterialLots
            .AsNoTracking()
            .Where(lot => ids.Contains(lot.Id) && lot.Material.IsActive)
            .Select(lot => new { lot.Id, MaterialCode = lot.Material.Code, MaterialName = lot.Material.Name, lot.LotNo, lot.ExpiryDate })
            .ToDictionaryAsync(lot => lot.Id, cancellationToken);

        var today = _clock.Today();
        var lots = new List<PlannedLot>();
        for (var index = 0; index < request.Materials.Count; index++)
        {
            var entry = request.Materials[index];
            if (!found.TryGetValue(entry.MaterialLotId, out var lot) || lots.Any(item => item.LotId == lot.Id))
            {
                errors.Add(new FieldError($"Materials[{index}].MaterialLotId", "Lot seçilemez ya da aynı lot iki kez eklenmiş."));
                continue;
            }

            var expired = _calculator.IsLotExpired(lot.ExpiryDate, today);
            var reason = entry.ExpiredReason?.Trim();
            if (expired && string.IsNullOrEmpty(reason))
            {
                errors.Add(new FieldError($"Materials[{index}].ExpiredReason", "SKT geçmiş lot için gerekçe girin."));
            }
            else if (reason?.Length > ReasonMaxLength)
            {
                errors.Add(new FieldError($"Materials[{index}].ExpiredReason", $"Gerekçe en çok {ReasonMaxLength} karakter olabilir."));
            }

            lots.Add(new PlannedLot(lot.Id, lot.MaterialCode, lot.MaterialName, lot.LotNo, lot.ExpiryDate, expired, expired ? reason : null));
        }

        return lots;
    }

    private sealed record ChosenType(int CleaningTypeId, int VersionId, int VersionNo);
}
