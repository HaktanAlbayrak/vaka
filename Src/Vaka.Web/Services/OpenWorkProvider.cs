using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.ViewModels.Users;

namespace Vaka.Web.Services;

public sealed class OpenWorkProvider : IOpenWorkProvider
{
    private readonly VakaDbContext _db;

    public OpenWorkProvider(VakaDbContext db)
    {
        _db = db;
    }

    public async Task<OpenWorkItemViewModel[]> ListAsync(int targetUserId, int viewerUserId, CancellationToken cancellationToken)
    {
        var roles = await UserQueries.GetRolesAsync(_db, viewerUserId, cancellationToken);
        var owned = await _db.CleaningRecords
            .AsNoTracking()
            .Where(record => record.OwnerUserId == targetUserId
                && (record.Status == RecordStatus.Opened || record.Status == RecordStatus.InProgress || record.Status == RecordStatus.OnHold))
            .OrderBy(record => record.OpenedAtUtc)
            .Select(record => new { record.Id, record.RecordNo, record.Status })
            .ToListAsync(cancellationToken);

        var assigned = await _db.StepParticipants
            .AsNoTracking()
            .Where(participant => participant.UserId == targetUserId
                && participant.Step.Phase.Record.OwnerUserId != targetUserId
                && (participant.Step.Status == StepStatus.Waiting || participant.Step.Status == StepStatus.InProgress)
                && (participant.Step.Phase.Record.Status == RecordStatus.Opened
                    || participant.Step.Phase.Record.Status == RecordStatus.InProgress
                    || participant.Step.Phase.Record.Status == RecordStatus.OnHold))
            .Select(participant => new { Id = participant.Step.Phase.CleaningRecordId, participant.Step.Phase.Record.RecordNo })
            .Distinct()
            .ToListAsync(cancellationToken);

        var onBreak = await _db.RecordBreaks
            .AsNoTracking()
            .Where(item => item.StartedByUserId == targetUserId && item.EndedAtUtc == null)
            .Select(item => new { Id = item.CleaningRecordId, item.Record.RecordNo })
            .ToListAsync(cancellationToken);

        return
        [
            .. owned.Select(row => new OpenWorkItemViewModel
            {
                Kind = "Kayıt sahibi",
                Title = $"{row.RecordNo} ({RecordLabels.Status(row.Status).ToLowerInvariant()})",
                Url = RecordLinks.For(roles, row.Id)
            }),
            .. assigned.Select(row => new OpenWorkItemViewModel
            {
                Kind = "Görevli adım",
                Title = row.RecordNo,
                Url = RecordLinks.For(roles, row.Id)
            }),
            .. onBreak.Select(row => new OpenWorkItemViewModel
            {
                Kind = "Açık mola",
                Title = row.RecordNo,
                Url = RecordLinks.For(roles, row.Id)
            })
        ];
    }
}
