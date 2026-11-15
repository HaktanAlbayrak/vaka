using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;

namespace Vaka.Web.Services;

public sealed class RecordBlockFinder : IRecordBlockFinder
{
    private readonly VakaDbContext _db;
    private readonly ITurkeyClock _clock;

    public RecordBlockFinder(VakaDbContext db, ITurkeyClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<OpenOutcome?> FindAsync(int machineId, int ownerUserId, CancellationToken cancellationToken)
    {
        var onMachine = await _db.CleaningRecords
            .AsNoTracking()
            .Where(record => record.MachineId == machineId
                && (record.Status == RecordStatus.Opened || record.Status == RecordStatus.InProgress || record.Status == RecordStatus.OnHold))
            .Select(record => new { record.Id, OwnerName = record.Owner.FullName, record.Status, record.OpenedAtUtc })
            .FirstOrDefaultAsync(cancellationToken);

        if (onMachine is not null)
        {
            var summary = RecordLabels.ActiveRecordSummary(onMachine.OwnerName, onMachine.Status, _clock.ToLocal(onMachine.OpenedAtUtc));
            return new OpenOutcome(CommandResult.Rejected(string.Empty, "Makinede açık kayıt var. " + summary), onMachine.Id);
        }

        var own = await _db.CleaningRecords
            .AsNoTracking()
            .Where(record => record.OwnerUserId == ownerUserId
                && (record.Status == RecordStatus.Opened || record.Status == RecordStatus.InProgress))
            .Select(record => new { record.Id, record.RecordNo, record.Status })
            .FirstOrDefaultAsync(cancellationToken);

        return own is null
            ? null
            : new OpenOutcome(
                CommandResult.Rejected(string.Empty, $"Açık kaydınız var: {own.RecordNo} ({RecordLabels.Status(own.Status).ToLowerInvariant()}). Yeni kayıt açmadan önce onu sürdürün."),
                own.Id);
    }
}
