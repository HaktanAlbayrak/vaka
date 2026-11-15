using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;

namespace Vaka.Web.Services;

public sealed class RecordLotChecker : IRecordLotChecker
{
    private const int ReasonMaxLength = 500;

    private readonly VakaDbContext _db;
    private readonly IDurationCalculator _calculator;
    private readonly ITurkeyClock _clock;

    public RecordLotChecker(VakaDbContext db, IDurationCalculator calculator, ITurkeyClock clock)
    {
        _db = db;
        _calculator = calculator;
        _clock = clock;
    }

    public async Task<IReadOnlyList<PlannedLot>> CheckAsync(IReadOnlyList<LotEntryRequest> entries, List<FieldError> errors, CancellationToken cancellationToken)
    {
        var ids = entries.Select(item => item.MaterialLotId).Distinct().ToArray();
        var found = await _db.MaterialLots
            .AsNoTracking()
            .Where(lot => ids.Contains(lot.Id) && lot.Material.IsActive)
            .Select(lot => new { lot.Id, MaterialCode = lot.Material.Code, MaterialName = lot.Material.Name, lot.LotNo, lot.ExpiryDate })
            .ToDictionaryAsync(lot => lot.Id, cancellationToken);

        var today = _clock.Today();
        var lots = new List<PlannedLot>();
        for (var index = 0; index < entries.Count; index++)
        {
            var entry = entries[index];
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
}
