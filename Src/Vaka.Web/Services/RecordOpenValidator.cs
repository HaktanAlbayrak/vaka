using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;

namespace Vaka.Web.Services;

public sealed class RecordOpenValidator : IRecordOpenValidator
{
    private const int WorkOrderMaxLength = 64;
    private const int NoteMaxLength = 1000;

    private readonly VakaDbContext _db;
    private readonly IDurationCalculator _calculator;
    private readonly ISystemSettingsReader _settings;
    private readonly IRecordLotChecker _lots;
    private readonly IRecordMachineChecker _machines;

    public RecordOpenValidator(VakaDbContext db, IDurationCalculator calculator, ISystemSettingsReader settings, IRecordLotChecker lots, IRecordMachineChecker machines)
    {
        _db = db;
        _calculator = calculator;
        _settings = settings;
        _lots = lots;
        _machines = machines;
    }

    public async Task<OpenValidation> ValidateAsync(RecordOpenRequest request, CancellationToken cancellationToken)
    {
        var errors = new List<FieldError>();
        var machine = await _machines.CheckAsync(request.FacilityId, request.LineId, request.MachineId, errors, cancellationToken);
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

        var lots = await _lots.CheckAsync(request.Materials, errors, cancellationToken);
        if (errors.Count > 0 || machine is null || type is null)
        {
            return new OpenValidation(null, errors);
        }

        return new OpenValidation(
            new RecordOpenPlan(machine, type.CleaningTypeId, type.VersionId, type.VersionNo, workOrder, note, request.LockMinutes, lots),
            errors);
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

    private sealed record ChosenType(int CleaningTypeId, int VersionId, int VersionNo);
}
