using Microsoft.EntityFrameworkCore;
using Vaka.Web.Authorization;
using Vaka.Web.Data;
using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed class ParticipantDirectory : IParticipantDirectory
{
    private const int PageSize = 50;

    private readonly VakaDbContext _db;

    public ParticipantDirectory(VakaDbContext db)
    {
        _db = db;
    }

    public async Task<ParticipantSearchResult> SearchAsync(string? query, int? stepId, int? includeUserId, CancellationToken cancellationToken)
    {
        var users = _db.Users.AsNoTracking().Where(user => user.Roles.Any(role => role.Role == Roles.Operator));
        var term = query?.Trim();
        if (!string.IsNullOrEmpty(term))
        {
            users = users.Where(user => user.FullName.Contains(term) || user.RegistryNo.Contains(term));
        }

        var rows = await users
            .OrderBy(user => user.FullName).ThenBy(user => user.Id)
            .Take(PageSize + 1)
            .Select(user => new CandidateRow(user.Id, user.FullName, user.RegistryNo, user.IsActive))
            .ToListAsync(cancellationToken);
        var truncated = rows.Count > PageSize;
        rows = rows.Take(PageSize).ToList();

        if (includeUserId is { } extra && rows.All(row => row.Id != extra))
        {
            var pending = await _db.Users
                .AsNoTracking()
                .Where(user => user.Id == extra && user.Roles.Any(role => role.Role == Roles.Operator))
                .Select(user => new CandidateRow(user.Id, user.FullName, user.RegistryNo, user.IsActive))
                .FirstOrDefaultAsync(cancellationToken);
            if (pending is not null)
            {
                rows.Add(pending);
            }
        }

        var ids = rows.Select(row => row.Id).ToArray();
        var inStep = stepId is { } step
            ? await _db.StepParticipants.AsNoTracking().Where(item => item.RecordStepId == step && ids.Contains(item.UserId)).Select(item => item.UserId).ToListAsync(cancellationToken)
            : [];
        var onBreak = await _db.RecordBreaks.AsNoTracking().Where(item => item.EndedAtUtc == null && ids.Contains(item.StartedByUserId)).Select(item => item.StartedByUserId).ToListAsync(cancellationToken);
        var running = await _db.CleaningRecords.AsNoTracking().Where(item => item.Status == RecordStatus.InProgress && ids.Contains(item.OwnerUserId)).Select(item => item.OwnerUserId).ToListAsync(cancellationToken);

        var options = rows.Select(row =>
        {
            var reason = !row.IsActive ? "Pasif kullanıcı"
                : inStep.Contains(row.Id) ? "Adımda zaten görevli"
                : onBreak.Contains(row.Id) ? "Molada"
                : null;
            return new ParticipantOptionViewModel
            {
                UserId = row.Id,
                FullName = row.FullName,
                RegistryNo = row.RegistryNo,
                IsAvailable = reason is null,
                UnavailableReason = reason,
                HasOwnActiveRecord = running.Contains(row.Id)
            };
        });

        return new ParticipantSearchResult([.. options], truncated);
    }

    private sealed record CandidateRow(int Id, string FullName, string RegistryNo, bool IsActive);
}
