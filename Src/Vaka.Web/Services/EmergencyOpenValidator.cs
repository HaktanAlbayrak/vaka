using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;

namespace Vaka.Web.Services;

public sealed class EmergencyOpenValidator : IEmergencyOpenValidator
{
    private const int DescriptionMaxLength = 1000;

    private readonly VakaDbContext _db;
    private readonly IRecordMachineChecker _machines;
    private readonly IRecordLotChecker _lots;

    public EmergencyOpenValidator(VakaDbContext db, IRecordMachineChecker machines, IRecordLotChecker lots)
    {
        _db = db;
        _machines = machines;
        _lots = lots;
    }

    public async Task<EmergencyValidation> ValidateAsync(EmergencyOpenRequest request, CancellationToken cancellationToken)
    {
        var errors = new List<FieldError>();
        var machine = await _machines.CheckAsync(request.FacilityId, request.LineId, request.MachineId, errors, cancellationToken);

        var type = request.CleaningTypeId is null
            ? null
            : await _db.ProcedureVersions
                .AsNoTracking()
                .Where(version => version.CleaningTypeId == request.CleaningTypeId
                    && version.Status == ProcedureVersionStatus.Current
                    && version.CleaningType.IsActive
                    && version.CleaningType.Kind == CleaningTypeKind.Emergency)
                .Select(version => new { version.CleaningTypeId, VersionId = version.Id, version.VersionNo, version.MaterialRequired })
                .FirstOrDefaultAsync(cancellationToken);
        if (type is null)
        {
            errors.Add(new FieldError("CleaningTypeId", "Acil tipi seçin."));
        }

        var description = request.Description?.Trim() ?? string.Empty;
        if (description.Length == 0)
        {
            errors.Add(new FieldError("Description", "Ne olduğunu kısaca yazın."));
        }
        else if (description.Length > DescriptionMaxLength)
        {
            errors.Add(new FieldError("Description", $"Açıklama en çok {DescriptionMaxLength} karakter olabilir."));
        }

        var lots = await _lots.CheckAsync(request.Materials, errors, cancellationToken);
        if (type is { MaterialRequired: true } && request.Materials.Count == 0)
        {
            errors.Add(new FieldError("Materials", "Bu acil tip malzeme ister. En az bir malzeme ekleyin."));
        }

        if (errors.Count > 0 || machine is null || type is null)
        {
            return new EmergencyValidation(null, errors);
        }

        return new EmergencyValidation(new EmergencyPlan(machine, type.CleaningTypeId, type.VersionId, type.VersionNo, description, lots), errors);
    }
}
