using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed class LotCatalog : ILotCatalog
{
    private const int PageSize = 50;

    private readonly VakaDbContext _db;
    private readonly IDurationCalculator _calculator;
    private readonly ITurkeyClock _clock;

    public LotCatalog(VakaDbContext db, IDurationCalculator calculator, ITurkeyClock clock)
    {
        _db = db;
        _calculator = calculator;
        _clock = clock;
    }

    public async Task<LotSearchResult> FindAsync(string? query, int? includeLotId, int? excludeRecordId, CancellationToken cancellationToken)
    {
        var lots = _db.MaterialLots.AsNoTracking().Where(lot => lot.Material.IsActive);
        var attachedLotIds = new List<int>();
        if (excludeRecordId is { } recordId)
        {
            attachedLotIds = await _db.RecordMaterials
                .AsNoTracking()
                .Where(material => material.CleaningRecordId == recordId && material.RemovedAtUtc == null)
                .Select(material => material.MaterialLotId)
                .ToListAsync(cancellationToken);
            lots = lots.Where(lot => !attachedLotIds.Contains(lot.Id));
        }

        var term = query?.Trim();
        if (!string.IsNullOrEmpty(term))
        {
            lots = lots.Where(lot => lot.LotNo.Contains(term) || lot.Material.Code.Contains(term) || lot.Material.Name.Contains(term));
        }

        var rows = await Project(lots.OrderBy(lot => lot.Material.Code).ThenBy(lot => lot.LotNo).Take(PageSize + 1))
            .ToListAsync(cancellationToken);
        var truncated = rows.Count > PageSize;
        var result = Map(rows.Take(PageSize)).ToList();

        if (includeLotId is { } extra && result.All(lot => lot.Id != extra) && !attachedLotIds.Contains(extra))
        {
            result.AddRange(await GetAsync([extra], cancellationToken));
        }

        return new LotSearchResult([.. result], truncated);
    }

    public async Task<LotOptionViewModel[]> GetAsync(IReadOnlyCollection<int> lotIds, CancellationToken cancellationToken)
    {
        var rows = await Project(_db.MaterialLots.AsNoTracking().Where(lot => lotIds.Contains(lot.Id))).ToListAsync(cancellationToken);
        return [.. Map(rows)];
    }

    private static IQueryable<LotRow> Project(IQueryable<Domain.Entities.MaterialLot> lots) =>
        lots.Select(lot => new LotRow(lot.Id, lot.Material.Code, lot.Material.Name, lot.LotNo, lot.ExpiryDate));

    private IEnumerable<LotOptionViewModel> Map(IEnumerable<LotRow> rows)
    {
        var today = _clock.Today();
        return rows.Select(row => new LotOptionViewModel
        {
            Id = row.Id,
            MaterialCode = row.MaterialCode,
            MaterialName = row.MaterialName,
            LotNo = row.LotNo,
            ExpiryDate = row.ExpiryDate,
            IsExpired = _calculator.IsLotExpired(row.ExpiryDate, today)
        });
    }

    private sealed record LotRow(int Id, string MaterialCode, string MaterialName, string LotNo, DateOnly ExpiryDate);
}
