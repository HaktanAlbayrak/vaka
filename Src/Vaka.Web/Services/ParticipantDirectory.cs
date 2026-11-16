using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Vaka.Web.Authorization;
using Vaka.Web.Data;
using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed class ParticipantDirectory : IParticipantDirectory
{
    private const int PageSize = 50;

    private static readonly CultureInfo Turkish = CultureInfo.GetCultureInfo("tr-TR");

    private readonly VakaDbContext _db;

    public ParticipantDirectory(VakaDbContext db)
    {
        _db = db;
    }

    public async Task<LookupResult<ParticipantSearchResult>> SearchForOwnerAsync(int stepId, int actorUserId, string? query, CancellationToken cancellationToken)
    {
        var ownerId = await _db.RecordSteps
            .AsNoTracking()
            .Where(step => step.Id == stepId)
            .Select(step => (int?)step.Phase.Record.OwnerUserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (ownerId is null)
        {
            return LookupResult<ParticipantSearchResult>.Missing();
        }

        return ownerId != actorUserId
            ? LookupResult<ParticipantSearchResult>.OutOfScope()
            : LookupResult<ParticipantSearchResult>.Found(await SearchAsync(query, stepId, null, cancellationToken));
    }

    public async Task<ParticipantSearchResult> SearchAsync(string? query, int? stepId, int? includeUserId, CancellationToken cancellationToken)
    {
        // SQLite karşılaştırması Türkçe harfte büyük-küçük harfe duyarlıdır; aday sayısı küçük olduğundan ad ve sicil no bellekte süzülür.
        var candidates = _db.Users
            .AsNoTracking()
            .Where(user => user.IsActive && user.Roles.Any(role => role.Role == Roles.Operator));
        if (stepId is { } step)
        {
            candidates = candidates.Where(user => !_db.StepParticipants.Any(item => item.RecordStepId == step && item.UserId == user.Id));
        }

        var rows = await candidates
            .Select(user => new CandidateRow(user.Id, user.FullName, user.RegistryNo))
            .ToListAsync(cancellationToken);

        var term = query?.Trim();
        var matched = rows
            .Where(row => string.IsNullOrEmpty(term) || Contains(row.FullName, term) || Contains(row.RegistryNo, term))
            .OrderBy(row => row.FullName, StringComparer.Create(Turkish, true)).ThenBy(row => row.Id)
            .ToList();
        var truncated = matched.Count > PageSize;
        var page = matched.Take(PageSize).ToList();

        if (includeUserId is { } extra && page.All(row => row.Id != extra) && rows.FirstOrDefault(row => row.Id == extra) is { } pending)
        {
            page.Add(pending);
        }

        var ids = page.Select(row => row.Id).ToArray();
        var onBreak = await _db.RecordBreaks.AsNoTracking().Where(item => item.EndedAtUtc == null && ids.Contains(item.StartedByUserId)).Select(item => item.StartedByUserId).ToListAsync(cancellationToken);
        var running = await _db.CleaningRecords.AsNoTracking().Where(item => item.Status == RecordStatus.InProgress && ids.Contains(item.OwnerUserId)).Select(item => item.OwnerUserId).ToListAsync(cancellationToken);

        var options = page.Select(row => new ParticipantOptionViewModel
        {
            UserId = row.Id,
            FullName = row.FullName,
            RegistryNo = row.RegistryNo,
            IsAvailable = !onBreak.Contains(row.Id),
            UnavailableReason = onBreak.Contains(row.Id) ? "Molada" : null,
            HasOwnActiveRecord = running.Contains(row.Id)
        });

        return new ParticipantSearchResult([.. options], truncated);
    }

    private static bool Contains(string text, string term) =>
        Turkish.CompareInfo.IndexOf(text, term, CompareOptions.IgnoreCase) >= 0;

    private sealed record CandidateRow(int Id, string FullName, string RegistryNo);
}
