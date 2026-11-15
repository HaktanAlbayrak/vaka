using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;
using Vaka.Web.ViewModels.Definitions;
using Vaka.Web.ViewModels.Reports;

namespace Vaka.Web.Services;

public sealed class MaterialTraceReader : IMaterialTraceReader
{
    private readonly VakaDbContext _db;
    private readonly IRecordTimeouts _timeouts;
    private readonly ILotCatalog _lots;
    private readonly IReportOptions _options;
    private readonly ITurkeyClock _clock;

    public MaterialTraceReader(VakaDbContext db, IRecordTimeouts timeouts, ILotCatalog lots, IReportOptions options, ITurkeyClock clock)
    {
        _db = db;
        _timeouts = timeouts;
        _lots = lots;
        _options = options;
        _clock = clock;
    }

    public async Task<MaterialTraceViewModel> BuildAsync(MaterialTraceViewModel request, string? inputError, CancellationToken cancellationToken)
    {
        await _timeouts.ApplyAsync(TimeoutScope.Everything, cancellationToken);

        var modeValid = Enum.IsDefined(request.Mode);
        var mode = modeValid ? request.Mode : MaterialTraceMode.Lot;
        var model = new MaterialTraceViewModel
        {
            Mode = mode,
            LotQuery = request.LotQuery?.Trim(),
            MaterialLotId = request.MaterialLotId,
            MachineId = request.MachineId,
            From = request.From,
            To = request.To,
            Machines = await _options.ListAllMachinesAsync(cancellationToken),
            ModeOptions =
            [
                new SelectOptionViewModel { Id = (int)MaterialTraceMode.Lot, Label = "Lota göre" },
                new SelectOptionViewModel { Id = (int)MaterialTraceMode.Machine, Label = "Makineye göre" }
            ]
        };

        if (inputError is not null)
        {
            model.ErrorMessage = inputError;
        }
        else if (!modeValid)
        {
            model.ErrorMessage = ReportMessages.UnknownMode;
        }
        else if (model.From > model.To)
        {
            model.ErrorMessage = ReportMessages.InvalidRange;
        }
        else if (mode == MaterialTraceMode.Machine && model.MachineId is { } machineId
            && await _options.FindInvalidSelectionAsync(null, null, machineId, null, cancellationToken) is { } unknown)
        {
            model.ErrorMessage = unknown;
        }
        else if (mode == MaterialTraceMode.Lot && model.MaterialLotId is { } lotId
            && !await _db.MaterialLots.AnyAsync(item => item.Id == lotId, cancellationToken))
        {
            model.ErrorMessage = ReportMessages.UnknownLot;
        }

        if (mode == MaterialTraceMode.Lot)
        {
            var catalog = await _lots.FindAsync(model.LotQuery, model.MaterialLotId, null, cancellationToken);
            model.LotOptions = catalog.Lots;
            model.LotOptionsTruncated = catalog.Truncated;
            if (model.ErrorMessage is null && model.MaterialLotId is { } selectedLot)
            {
                model.HasResult = true;
                model.SelectedLotLabel = await ReadLotLabelAsync(selectedLot, cancellationToken);
                model.LotUsages = await ReadLotUsagesAsync(selectedLot, model, cancellationToken);
            }
        }
        else if (model.ErrorMessage is null && model.MachineId is { } selectedMachine)
        {
            model.HasResult = true;
            model.MachineMaterials = await ReadMachineMaterialsAsync(selectedMachine, model, cancellationToken);
        }

        return model;
    }

    private async Task<string?> ReadLotLabelAsync(int lotId, CancellationToken cancellationToken)
    {
        var lot = (await _lots.GetAsync([lotId], cancellationToken)).FirstOrDefault();
        return lot is null ? null : $"{lot.MaterialCode} - {lot.MaterialName}, lot {lot.LotNo}";
    }

    private async Task<LotUsageViewModel[]> ReadLotUsagesAsync(int lotId, MaterialTraceViewModel range, CancellationToken cancellationToken)
    {
        var materials = _db.RecordMaterials.AsNoTracking().Where(material => material.MaterialLotId == lotId && material.RemovedAtUtc == null);
        if (range.From is { } from)
        {
            var start = _clock.StartOfDayUtc(from);
            materials = materials.Where(material => material.Record.OpenedAtUtc >= start);
        }

        if (range.To is { } to)
        {
            var end = _clock.StartOfDayUtc(to.AddDays(1));
            materials = materials.Where(material => material.Record.OpenedAtUtc < end);
        }

        var rows = await materials
            .OrderBy(material => material.Record.OpenedAtUtc).ThenBy(material => material.CleaningRecordId)
            .Select(material => new
            {
                material.CleaningRecordId,
                material.Record.RecordNo,
                MachineLabel = material.Record.FacilityCodeSnapshot + "-" + material.Record.LineCodeSnapshot + "-" + material.Record.MachineCodeSnapshot + " - " + material.Record.MachineNameSnapshot,
                material.Record.OpenedAtUtc,
                material.Record.Status,
                material.ExpiredAtEntry
            })
            .ToListAsync(cancellationToken);

        return [.. rows.Select(row => new LotUsageViewModel
        {
            RecordId = row.CleaningRecordId,
            RecordNo = row.RecordNo,
            MachineLabel = row.MachineLabel,
            OpenedAtLocal = _clock.ToLocal(row.OpenedAtUtc),
            StatusLabel = RecordLabels.Status(row.Status),
            ExpiredAtEntry = row.ExpiredAtEntry,
            Url = RecordLinks.Details(row.CleaningRecordId)
        })];
    }

    private async Task<MachineMaterialViewModel[]> ReadMachineMaterialsAsync(int machineId, MaterialTraceViewModel range, CancellationToken cancellationToken)
    {
        var materials = _db.RecordMaterials.AsNoTracking().Where(material => material.Record.MachineId == machineId && material.RemovedAtUtc == null);
        if (range.From is { } from)
        {
            var start = _clock.StartOfDayUtc(from);
            materials = materials.Where(material => material.EnteredAtUtc >= start);
        }

        if (range.To is { } to)
        {
            var end = _clock.StartOfDayUtc(to.AddDays(1));
            materials = materials.Where(material => material.EnteredAtUtc < end);
        }

        var rows = await materials
            .OrderBy(material => material.EnteredAtUtc).ThenBy(material => material.Id)
            .Select(material => new
            {
                material.EnteredAtUtc,
                material.CleaningRecordId,
                material.Record.RecordNo,
                material.MaterialCodeSnapshot,
                material.MaterialNameSnapshot,
                material.LotNoSnapshot,
                material.ExpiryDateSnapshot,
                material.ExpiredAtEntry
            })
            .ToListAsync(cancellationToken);

        return [.. rows.Select(row => new MachineMaterialViewModel
        {
            EnteredAtLocal = _clock.ToLocal(row.EnteredAtUtc),
            RecordId = row.CleaningRecordId,
            RecordNo = row.RecordNo,
            MaterialCode = row.MaterialCodeSnapshot,
            MaterialName = row.MaterialNameSnapshot,
            LotNo = row.LotNoSnapshot,
            ExpiryDate = row.ExpiryDateSnapshot,
            ExpiredAtEntry = row.ExpiredAtEntry,
            Url = RecordLinks.Details(row.CleaningRecordId)
        })];
    }
}
