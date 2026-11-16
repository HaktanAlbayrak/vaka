using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;
using Vaka.Web.ViewModels.Reports;

namespace Vaka.Web.Services;

public sealed class MaterialUsageReader : IMaterialUsageReader
{
    private readonly VakaDbContext _db;
    private readonly ITurkeyClock _clock;

    public MaterialUsageReader(VakaDbContext db, ITurkeyClock clock)
    {
        _db = db;
        _clock = clock;
    }

    /// <summary>
    /// Kullanım oranı (%) = malzemenin en az bir lotuyla yer aldığı Tamamlandı kayıt sayısı / süzgece uyan Tamamlandı kayıt sayısı x 100;
    /// bir ondalığa yuvarlanır (0,05 yukarı). Payda 0 ise oran ve satır yoktur.
    /// </summary>
    public async Task<MaterialUsageViewModel> ReadAsync(MaterialUsageFilter filter, CancellationToken cancellationToken)
    {
        var completed = Filter(_db.CleaningRecords.AsNoTracking().Where(record => record.Status == RecordStatus.Completed), filter);
        var completedCount = await completed.CountAsync(cancellationToken);
        if (completedCount == 0)
        {
            return new MaterialUsageViewModel();
        }

        var ids = completed.Select(record => record.Id);
        var uses = await _db.RecordMaterials
            .AsNoTracking()
            .Where(material => material.RemovedAtUtc == null && ids.Contains(material.CleaningRecordId))
            .Select(material => new
            {
                material.MaterialLot.MaterialId,
                material.MaterialLot.Material.Code,
                material.MaterialLot.Material.Name,
                material.CleaningRecordId,
                material.MaterialLotId,
                material.ExpiredAtEntry
            })
            .ToListAsync(cancellationToken);

        var rows = uses
            .GroupBy(use => use.MaterialId)
            .Select(group =>
            {
                var recordCount = group.Select(use => use.CleaningRecordId).Distinct().Count();
                return new MaterialUsageRowViewModel
                {
                    MaterialCode = group.First().Code,
                    MaterialName = group.First().Name,
                    RecordCount = recordCount,
                    UsageRatePercent = Math.Round(recordCount * 100m / completedCount, 1, MidpointRounding.AwayFromZero),
                    LotCount = group.Select(use => use.MaterialLotId).Distinct().Count(),
                    ExpiredUseCount = group.Count(use => use.ExpiredAtEntry)
                };
            })
            .OrderByDescending(row => row.RecordCount).ThenBy(row => row.MaterialCode, StringComparer.CurrentCulture);

        return new MaterialUsageViewModel
        {
            CompletedRecordCount = completedCount,
            RecordsWithMaterialCount = uses.Select(use => use.CleaningRecordId).Distinct().Count(),
            Rows = [.. rows]
        };
    }

    private IQueryable<CleaningRecord> Filter(IQueryable<CleaningRecord> records, MaterialUsageFilter filter)
    {
        if (filter.FacilityId is { } facilityId)
        {
            records = records.Where(record => record.Machine.Line.FacilityId == facilityId);
        }

        if (filter.LineId is { } lineId)
        {
            records = records.Where(record => record.Machine.ProductionLineId == lineId);
        }

        if (filter.MachineId is { } machineId)
        {
            records = records.Where(record => record.MachineId == machineId);
        }

        if (filter.CleaningTypeId is { } typeId)
        {
            records = records.Where(record => record.ProcedureVersion.CleaningTypeId == typeId);
        }

        if (filter.Kind is { } kind)
        {
            records = records.Where(record => record.Kind == kind);
        }

        if (filter.From is { } from)
        {
            var start = _clock.StartOfDayUtc(from);
            records = records.Where(record => record.OpenedAtUtc >= start);
        }

        if (filter.To is { } to)
        {
            var end = _clock.StartOfDayUtc(to.AddDays(1));
            records = records.Where(record => record.OpenedAtUtc < end);
        }

        return records;
    }
}
