using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed class MyWorkReader : IMyWorkReader
{
    private readonly VakaDbContext _db;
    private readonly IRecordTimeouts _timeouts;
    private readonly ITurkeyClock _clock;

    public MyWorkReader(VakaDbContext db, IRecordTimeouts timeouts, ITurkeyClock clock)
    {
        _db = db;
        _timeouts = timeouts;
        _clock = clock;
    }

    public async Task<MyWorkViewModel> GetAsync(int userId, CancellationToken cancellationToken)
    {
        await _timeouts.ApplyAsync(TimeoutScope.ForOwner(userId), cancellationToken);

        var row = await _db.CleaningRecords
            .AsNoTracking()
            .Where(record => record.OwnerUserId == userId && (record.Status == RecordStatus.Opened || record.Status == RecordStatus.InProgress))
            .Select(record => new
            {
                record.Id,
                record.RecordNo,
                record.Kind,
                record.Status,
                Machine = record.MachineCodeSnapshot + " - " + record.MachineNameSnapshot,
                TypeName = record.ProcedureVersion.CleaningType.Name,
                record.ProcedureVersion.VersionNo,
                record.LockExpiresAtUtc
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (row is null)
        {
            return new MyWorkViewModel { CanOpenRecord = true };
        }

        var url = RecordLinks.Run(row.Id);
        return new MyWorkViewModel
        {
            ActiveRecord = new ActiveRecordViewModel
            {
                Id = row.Id,
                RecordNo = row.RecordNo,
                Kind = row.Kind,
                KindLabel = ProcedureLabels.Kind(row.Kind),
                Status = row.Status,
                StatusLabel = RecordLabels.Status(row.Status),
                MachineLabel = row.Machine,
                CleaningTypeName = row.TypeName,
                VersionNo = row.VersionNo,
                LockExpiresAtLocal = row.Status == RecordStatus.Opened && row.LockExpiresAtUtc is { } expires ? _clock.ToLocal(expires) : null,
                Url = url
            },
            CanOpenRecord = false,
            CannotOpenReason = $"Açık kaydınız var: {row.RecordNo} ({RecordLabels.Status(row.Status).ToLowerInvariant()}). Yeni kayıt açmadan önce onu sürdürün.",
            CannotOpenUrl = url
        };
    }
}
