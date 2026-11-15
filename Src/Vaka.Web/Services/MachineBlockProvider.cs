using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Services;

public sealed class MachineBlockProvider : IMachineBlockProvider
{
    private readonly VakaDbContext _db;

    public MachineBlockProvider(VakaDbContext db)
    {
        _db = db;
    }

    public async Task<BlockingRecordViewModel[]> ListAsync(int machineId, int viewerUserId, CancellationToken cancellationToken)
    {
        var roles = await UserQueries.GetRolesAsync(_db, viewerUserId, cancellationToken);
        var rows = await _db.CleaningRecords
            .AsNoTracking()
            .Where(record => record.MachineId == machineId
                && (record.Status == RecordStatus.Opened || record.Status == RecordStatus.InProgress || record.Status == RecordStatus.OnHold))
            .OrderBy(record => record.OpenedAtUtc)
            .Select(record => new { record.Id, record.RecordNo, record.Status })
            .ToListAsync(cancellationToken);

        return [.. rows.Select(row => new BlockingRecordViewModel
        {
            RecordNo = row.RecordNo,
            StatusLabel = RecordLabels.Status(row.Status),
            Url = RecordLinks.For(roles, row.Id)
        })];
    }
}
