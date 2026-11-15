using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed class RecordCreateForm : IRecordCreateForm
{
    private readonly IRecordOptions _options;
    private readonly ILotCatalog _lots;
    private readonly IMaterialEntryResolver _materials;
    private readonly ISystemSettingsReader _settings;

    public RecordCreateForm(IRecordOptions options, ILotCatalog lots, IMaterialEntryResolver materials, ISystemSettingsReader settings)
    {
        _options = options;
        _lots = lots;
        _materials = materials;
        _settings = settings;
    }

    public async Task<RecordCreateViewModel> BuildAsync(RecordCreateViewModel? posted, bool applyMaterialEdits, CancellationToken cancellationToken)
    {
        var model = new RecordCreateViewModel
        {
            CommandId = posted is { CommandId: var id } && id != Guid.Empty ? id : Guid.NewGuid(),
            WorkOrderNo = posted?.WorkOrderNo ?? string.Empty,
            Note = posted?.Note ?? string.Empty,
            LockMinutes = posted?.LockMinutes ?? 0,
            LotQuery = posted?.LotQuery,
            ErrorMessage = posted?.ErrorMessage,
            ErrorUrl = posted?.ErrorUrl,
            MaxLockMinutes = await _settings.GetValueAsync(SettingKeys.MaxLockMinutes, cancellationToken),
            Facilities = await _options.ListFacilitiesAsync(cancellationToken)
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
            model.CleaningTypes = await _options.ListCleaningTypesAsync(machineId, cancellationToken);
        }

        if (posted?.CleaningTypeId is { } typeId && model.CleaningTypes.FirstOrDefault(item => item.Id == typeId) is { } type)
        {
            model.CleaningTypeId = typeId;
            model.DefaultLockMinutes = type.DefaultLockMinutes;
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
}
