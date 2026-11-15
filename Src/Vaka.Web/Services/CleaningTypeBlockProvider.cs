using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Services;

public sealed class CleaningTypeBlockProvider : ICleaningTypeBlockProvider
{
    private readonly VakaDbContext _db;

    public CleaningTypeBlockProvider(VakaDbContext db)
    {
        _db = db;
    }

    public async Task<BlockingRecordViewModel[]> ListAsync(int cleaningTypeId, int viewerUserId, CancellationToken cancellationToken)
    {
        var roles = await UserQueries.GetRolesAsync(_db, viewerUserId, cancellationToken);
        var rows = await _db.CleaningRecords
            .AsNoTracking()
            .Where(record => record.ProcedureVersion.CleaningTypeId == cleaningTypeId
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
