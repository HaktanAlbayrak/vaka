using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed class IntegrityReportReader : IIntegrityReportReader
{
    private const int HashDigits = 12;

    private readonly VakaDbContext _db;
    private readonly IRecordTimeouts _timeouts;
    private readonly IRecordIntegrity _integrity;
    private readonly ITurkeyClock _clock;
    private readonly TimeProvider _time;

    public IntegrityReportReader(VakaDbContext db, IRecordTimeouts timeouts, IRecordIntegrity integrity, ITurkeyClock clock, TimeProvider time)
    {
        _db = db;
        _timeouts = timeouts;
        _integrity = integrity;
        _clock = clock;
        _time = time;
    }

    public async Task<LookupResult<IntegrityReportViewModel>> BuildAsync(int recordId, CancellationToken cancellationToken)
    {
        await _timeouts.ApplyAsync(TimeoutScope.ForRecord(recordId), cancellationToken);

        var recordNo = await _db.CleaningRecords
            .AsNoTracking()
            .Where(record => record.Id == recordId)
            .Select(record => record.RecordNo)
            .FirstOrDefaultAsync(cancellationToken);
        var result = recordNo is null ? null : await _integrity.CheckAsync(recordId, cancellationToken);
        if (recordNo is null || result is null)
        {
            return LookupResult<IntegrityReportViewModel>.Missing();
        }

        return LookupResult<IntegrityReportViewModel>.Found(new IntegrityReportViewModel
        {
            RecordId = recordId,
            RecordNo = recordNo,
            IsValid = result.IsValid,
            CheckedAtLocal = _clock.ToLocal(_time.GetUtcNow().UtcDateTime),
            Events =
            [
                .. result.Events.Select(check => new IntegrityEventViewModel
                {
                    Sequence = check.Sequence,
                    EventLabel = RecordEventText.Label(check.EventType),
                    OccurredAtLocal = _clock.ToLocal(check.OccurredAtUtc),
                    HashShort = check.Hash.Length > HashDigits ? check.Hash[..HashDigits] : check.Hash,
                    IsValid = check.IsValid,
                    Problem = check.Problem
                })
            ],
            StateMismatches = [.. result.Mismatches]
        });
    }
}
