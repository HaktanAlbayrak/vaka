using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;

namespace Vaka.Web.Services;

public sealed class LoginAttemptGuard : ILoginAttemptGuard
{
    private readonly VakaDbContext _db;
    private readonly ISystemSettingsReader _settings;
    private readonly TimeProvider _time;

    public LoginAttemptGuard(VakaDbContext db, ISystemSettingsReader settings, TimeProvider time)
    {
        _db = db;
        _settings = settings;
        _time = time;
    }

    public async Task<DateTime?> GetLockedUntilAsync(int userId, CancellationToken cancellationToken)
    {
        var now = _time.GetUtcNow().UtcDateTime;
        var lockedUntil = await _db.Users
            .AsNoTracking()
            .Where(user => user.Id == userId)
            .Select(user => user.LoginLockedUntilUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (lockedUntil is null)
        {
            return null;
        }

        if (lockedUntil > now)
        {
            return lockedUntil;
        }

        await _db.Users
            .Where(user => user.Id == userId && user.LoginLockedUntilUtc <= now)
            .ExecuteUpdateAsync(update => update
                .SetProperty(user => user.LoginFailedCount, 0)
                .SetProperty(user => user.LoginLockedUntilUtc, (DateTime?)null), cancellationToken);
        return null;
    }

    public async Task<DateTime?> RegisterFailureAsync(int userId, CancellationToken cancellationToken)
    {
        await _db.Users
            .Where(user => user.Id == userId)
            .ExecuteUpdateAsync(update => update.SetProperty(user => user.LoginFailedCount, user => user.LoginFailedCount + 1), cancellationToken);

        var limit = await _settings.GetValueAsync(SettingKeys.PinAttemptLimit, cancellationToken);
        var minutes = await _settings.GetValueAsync(SettingKeys.LoginLockMinutes, cancellationToken);
        var until = _time.GetUtcNow().UtcDateTime.AddMinutes(minutes);

        var locked = await _db.Users
            .Where(user => user.Id == userId && user.LoginFailedCount >= limit && user.LoginLockedUntilUtc == null)
            .ExecuteUpdateAsync(update => update.SetProperty(user => user.LoginLockedUntilUtc, (DateTime?)until), cancellationToken);

        return locked > 0 ? until : null;
    }

    public async Task ResetAsync(int userId, CancellationToken cancellationToken) =>
        await _db.Users
            .Where(user => user.Id == userId && (user.LoginFailedCount != 0 || user.LoginLockedUntilUtc != null))
            .ExecuteUpdateAsync(update => update
                .SetProperty(user => user.LoginFailedCount, 0)
                .SetProperty(user => user.LoginLockedUntilUtc, (DateTime?)null), cancellationToken);
}
