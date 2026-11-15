using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.ViewModels.Breaks;

namespace Vaka.Web.Services;

public sealed class BreakReader : IBreakReader
{
    private readonly VakaDbContext _db;
    private readonly IDurationCalculator _calculator;
    private readonly ITurkeyClock _clock;
    private readonly TimeProvider _time;

    public BreakReader(VakaDbContext db, IDurationCalculator calculator, ITurkeyClock clock, TimeProvider time)
    {
        _db = db;
        _calculator = calculator;
        _clock = clock;
        _time = time;
    }

    public Task<bool> IsOnBreakAsync(int userId, CancellationToken cancellationToken) =>
        _db.RecordBreaks.AsNoTracking().AnyAsync(item => item.StartedByUserId == userId && item.EndedAtUtc == null, cancellationToken);

    public async Task<CurrentBreakViewModel?> GetCurrentAsync(int userId, CancellationToken cancellationToken)
    {
        var row = await _db.RecordBreaks
            .AsNoTracking()
            .Where(item => item.StartedByUserId == userId && item.EndedAtUtc == null)
            .Select(item => new
            {
                item.Id,
                item.CleaningRecordId,
                item.Record.RecordNo,
                Machine = item.Record.MachineCodeSnapshot + " - " + item.Record.MachineNameSnapshot,
                item.StartedAtUtc,
                DuringStepTitle = item.DuringStep == null ? null : item.DuringStep.ProcedureStep.Title
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (row is null)
        {
            return null;
        }

        return new CurrentBreakViewModel
        {
            BreakId = row.Id,
            RecordId = row.CleaningRecordId,
            RecordNo = row.RecordNo,
            MachineLabel = row.Machine,
            StartedAtLocal = _clock.ToLocal(row.StartedAtUtc),
            ElapsedSeconds = _calculator.CalculateBreakSeconds(UtcMoments.Of(row.StartedAtUtc), _time.GetUtcNow()),
            DuringStepTitle = row.DuringStepTitle,
            CommandId = Guid.NewGuid()
        };
    }
}
