using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.ViewModels.Emergencies;

namespace Vaka.Web.Services;

public sealed class EmergencyForm : IEmergencyForm
{
    private readonly VakaDbContext _db;
    private readonly IRecordOptions _options;
    private readonly ILotCatalog _lots;
    private readonly IMaterialEntryResolver _materials;

    public EmergencyForm(VakaDbContext db, IRecordOptions options, ILotCatalog lots, IMaterialEntryResolver materials)
    {
        _db = db;
        _options = options;
        _lots = lots;
        _materials = materials;
    }

    public async Task<EmergencyCreateViewModel> BuildAsync(EmergencyCreateViewModel? posted, bool applyMaterialEdits, CancellationToken cancellationToken)
    {
        var model = new EmergencyCreateViewModel
        {
            CommandId = posted is { CommandId: var id } && id != Guid.Empty ? id : Guid.NewGuid(),
            Description = posted?.Description ?? string.Empty,
            LotQuery = posted?.LotQuery,
            ErrorMessage = posted?.ErrorMessage,
            ErrorUrl = posted?.ErrorUrl,
            Facilities = await _options.ListFacilitiesAsync(cancellationToken),
            EmergencyTypes = await ListTypesAsync(cancellationToken)
        };

        if (posted?.FacilityId is { } facilityId && model.Facilities.Any(item => item.Id == facilityId))
        {
            model.FacilityId = facilityId;
            model.Lines = await _options.ListLinesAsync(facilityId, cancellationToken);
        }

        if (posted?.LineId is { } lineId && model.Lines.Any(item => item.Id == lineId))
        {
            model.LineId = lineId;
            model.Machines = await _options.ListMachinesAsync(lineId, cancellationToken);
        }

        if (posted?.MachineId is { } machineId && model.Machines.Any(item => item.Id == machineId))
        {
            model.MachineId = machineId;
        }

        if (posted?.CleaningTypeId is { } typeId && model.EmergencyTypes.Any(item => item.Id == typeId))
        {
            model.CleaningTypeId = typeId;
        }
        else if (model.EmergencyTypes.Length == 1)
        {
            model.CleaningTypeId = model.EmergencyTypes[0].Id;
        }

        MaterialEdits? edits = applyMaterialEdits && posted is not null
            ? new MaterialEdits(posted.RemoveMaterialIndex, posted.AddMaterialLotId, posted.AddExpiredReason)
            : null;
        model.Materials = await _materials.ResolveAsync(posted?.Materials ?? [], edits, cancellationToken);
        var catalog = await _lots.FindAsync(model.LotQuery, null, null, cancellationToken);
        model.AvailableLots = catalog.Lots;
        model.AvailableLotsTruncated = catalog.Truncated;
        return model;
    }

    private async Task<EmergencyTypeOptionViewModel[]> ListTypesAsync(CancellationToken cancellationToken) =>
        await _db.ProcedureVersions
            .AsNoTracking()
            .Where(version => version.Status == ProcedureVersionStatus.Current
                && version.CleaningType.IsActive
                && version.CleaningType.Kind == CleaningTypeKind.Emergency)
            .OrderBy(version => version.CleaningType.Name)
            .Select(version => new EmergencyTypeOptionViewModel
            {
                Id = version.CleaningTypeId,
                Name = version.CleaningType.Name,
                VersionNo = version.VersionNo,
                MaterialRequired = version.MaterialRequired
            })
            .ToArrayAsync(cancellationToken);
}
