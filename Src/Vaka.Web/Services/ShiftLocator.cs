using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;

namespace Vaka.Web.Services;

public sealed class ShiftLocator : IShiftLocator
{
    private readonly VakaDbContext _db;
    private readonly IDurationCalculator _calculator;

    public ShiftLocator(VakaDbContext db, IDurationCalculator calculator)
    {
        _db = db;
        _calculator = calculator;
    }

    public async Task<ShiftOccurrence?> FindAtAsync(int facilityId, DateTimeOffset moment, CancellationToken cancellationToken)
    {
        var windows = await _db.Shifts
            .AsNoTracking()
            .Where(shift => shift.FacilityId == facilityId && shift.IsActive)
            .Select(shift => new ShiftWindow(shift.Id, shift.StartTime, shift.EndTime))
            .ToListAsync(cancellationToken);

        return _calculator.FindShiftAt(moment, windows);
    }
}
