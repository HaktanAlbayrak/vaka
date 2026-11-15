using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

public sealed class RecordTimeouts : IRecordTimeouts
{
    private readonly VakaDbContext _db;
    private readonly IWriteTransactionRunner _transactions;
    private readonly ICleaningEventWriter _events;
    private readonly IDurationCalculator _calculator;
    private readonly TimeProvider _time;

    public RecordTimeouts(
        VakaDbContext db,
        IWriteTransactionRunner transactions,
        ICleaningEventWriter events,
        IDurationCalculator calculator,
        TimeProvider time)
    {
        _db = db;
        _transactions = transactions;
        _events = events;
        _calculator = calculator;
        _time = time;
    }

    public async Task ApplyAsync(TimeoutScope scope, CancellationToken cancellationToken)
    {
        var now = _time.GetUtcNow();
        var candidates = await _db.CleaningRecords
            .AsNoTracking()
            .Where(record => record.Status == RecordStatus.Opened && record.LockExpiresAtUtc != null)
            .Where(record => scope.All || record.Id == scope.RecordId || record.MachineId == scope.MachineId || record.OwnerUserId == scope.OwnerUserId)
            .Select(record => new { record.Id, record.LockExpiresAtUtc })
            .ToListAsync(cancellationToken);

        if (!candidates.Any(record => IsExpired(record.LockExpiresAtUtc!.Value, now)))
        {
            return;
        }

        var ids = candidates.Select(record => record.Id).ToArray();
        await _transactions.RunAsync(async token =>
        {
            var due = await _db.CleaningRecords
                .Where(record => ids.Contains(record.Id) && record.Status == RecordStatus.Opened && record.LockExpiresAtUtc != null)
                .ToListAsync(token);

            foreach (var record in due.Where(record => IsExpired(record.LockExpiresAtUtc!.Value, now)))
            {
                var expiresAt = record.LockExpiresAtUtc!.Value;
                record.Status = RecordStatus.TimedOut;
                record.ClosedAtUtc = expiresAt;
                record.Version++;
                await _db.SaveChangesAsync(token);
                await _events.AppendAsync(
                    new EventDraft(record.Id, EventTypes.RecordTimedOut, new { lockExpiresAtUtc = DateTime.SpecifyKind(expiresAt, DateTimeKind.Utc) }, null, null, now.UtcDateTime),
                    token);
            }

            return CommandResult.Success();
        }, cancellationToken);
    }

    private bool IsExpired(DateTime lockExpiresAtUtc, DateTimeOffset now) =>
        _calculator.IsLockExpired(new DateTimeOffset(DateTime.SpecifyKind(lockExpiresAtUtc, DateTimeKind.Utc)), now);
}
