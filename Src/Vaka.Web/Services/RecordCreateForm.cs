using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed class RecordCreateForm : IRecordCreateForm
{
    private readonly IRecordOptions _options;
    private readonly ILotCatalog _lots;
    private readonly ISystemSettingsReader _settings;

    public RecordCreateForm(IRecordOptions options, ILotCatalog lots, ISystemSettingsReader settings)
    {
        _options = options;
        _lots = lots;
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

        model.Materials = await ResolveMaterialsAsync(posted, applyMaterialEdits, cancellationToken);
        var catalog = await _lots.FindAsync(model.LotQuery, null, cancellationToken);
        model.AvailableLots = catalog.Lots;
        model.AvailableLotsTruncated = catalog.Truncated;
        return model;
    }

    private async Task<MaterialEntryViewModel[]> ResolveMaterialsAsync(RecordCreateViewModel? posted, bool applyMaterialEdits, CancellationToken cancellationToken)
    {
        var entries = (posted?.Materials ?? []).Select(item => (item.MaterialLotId, item.ExpiredReason)).ToList();
        if (applyMaterialEdits && posted is not null)
        {
            if (posted.RemoveMaterialIndex is { } index && index >= 0 && index < entries.Count)
            {
                entries.RemoveAt(index);
            }

            if (posted.AddMaterialLotId is { } lotId && entries.All(item => item.MaterialLotId != lotId))
            {
                entries.Add((lotId, posted.AddExpiredReason));
            }
        }

        var lots = (await _lots.GetAsync(entries.Select(item => item.MaterialLotId).ToArray(), cancellationToken))
            .ToDictionary(lot => lot.Id);

        return [.. entries.Where(item => lots.ContainsKey(item.MaterialLotId)).Select(item =>
        {
            var lot = lots[item.MaterialLotId];
            return new MaterialEntryViewModel
            {
                MaterialLotId = lot.Id,
                ExpiredReason = item.ExpiredReason,
                MaterialCode = lot.MaterialCode,
                MaterialName = lot.MaterialName,
                LotNo = lot.LotNo,
                ExpiryDate = lot.ExpiryDate,
                IsExpired = lot.IsExpired
            };
        })];
    }
}
