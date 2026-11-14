using Microsoft.EntityFrameworkCore;
using Vaka.Web.Authorization;
using Vaka.Web.Data;

namespace Vaka.Web.Services;

internal static class UserQueries
{
    public static async Task<string[]> GetRolesAsync(VakaDbContext db, int userId, CancellationToken cancellationToken) =>
        await db.UserRoles
            .AsNoTracking()
            .Where(role => role.UserId == userId)
            .Select(role => role.Role)
            .ToArrayAsync(cancellationToken);

    public static async Task<bool> IsActiveAsync(VakaDbContext db, int userId, CancellationToken cancellationToken) =>
        await db.Users.AsNoTracking().AnyAsync(user => user.Id == userId && user.IsActive, cancellationToken);

    /// <summary>PL-05: hedef aktif Süper admin ve başka aktif Süper admin yoksa true.</summary>
    public static async Task<bool> IsLastActiveSuperAdminAsync(VakaDbContext db, int userId, CancellationToken cancellationToken)
    {
        var activeSuperAdmins = await db.UserRoles
            .AsNoTracking()
            .Where(role => role.Role == Roles.SuperAdmin && role.User.IsActive)
            .Select(role => role.UserId)
            .ToListAsync(cancellationToken);

        return activeSuperAdmins.Count == 1 && activeSuperAdmins[0] == userId;
    }

    /// <summary>Kapsam ve açık iş dışındaki pasifleştirme engeli; engel yoksa boş (PL-05, PL-16).</summary>
    public static async Task<string?> GetDeactivateBlockAsync(
        VakaDbContext db,
        int actorId,
        int targetId,
        bool targetIsActive,
        CancellationToken cancellationToken)
    {
        if (!targetIsActive)
        {
            return "Hesap zaten pasif.";
        }

        if (actorId == targetId)
        {
            return "Kendi hesabınızı pasifleştiremezsiniz.";
        }

        return await IsLastActiveSuperAdminAsync(db, targetId, cancellationToken)
            ? "Son aktif Süper admin pasifleştirilemez."
            : null;
    }
}
