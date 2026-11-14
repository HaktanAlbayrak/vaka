using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

public sealed class PinService : IPinService
{
    private readonly VakaDbContext _db;
    private readonly ICredentialHasher _hasher;
    private readonly ISystemSettingsReader _settings;
    private readonly TimeProvider _time;

    public PinService(VakaDbContext db, ICredentialHasher hasher, ISystemSettingsReader settings, TimeProvider time)
    {
        _db = db;
        _hasher = hasher;
        _settings = settings;
        _time = time;
    }

    public async Task<PinCheckStatus> VerifyAsync(int userId, string? pin, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(candidate => candidate.Id == userId, cancellationToken);
        if (user is null || user.PinHash is null)
        {
            return PinCheckStatus.NotSet;
        }

        if (user.PinLockedAtUtc is not null)
        {
            return PinCheckStatus.Locked;
        }

        if (string.IsNullOrEmpty(pin))
        {
            return PinCheckStatus.Wrong;
        }

        if (_hasher.Verify(user.PinHash, pin))
        {
            if (user.PinFailedCount != 0)
            {
                user.PinFailedCount = 0;
                await _db.SaveChangesAsync(cancellationToken);
            }

            return PinCheckStatus.Accepted;
        }

        user.PinFailedCount++;
        var limit = await _settings.GetValueAsync(SettingKeys.PinAttemptLimit, cancellationToken);
        var lockedNow = user.PinFailedCount >= limit;
        if (lockedNow)
        {
            user.PinLockedAtUtc = _time.GetUtcNow().UtcDateTime;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return lockedNow ? PinCheckStatus.LockedNow : PinCheckStatus.Wrong;
    }

    public async Task SetAsync(int userId, string newPin, CancellationToken cancellationToken)
    {
        var user = await FindAsync(userId, cancellationToken);
        user.PinHash = _hasher.Hash(newPin);
        user.PinFailedCount = 0;
        user.PinLockedAtUtc = null;
        user.MustSetPin = false;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task ResetAsync(int userId, CancellationToken cancellationToken)
    {
        var user = await FindAsync(userId, cancellationToken);
        user.PinHash = null;
        user.PinFailedCount = 0;
        user.PinLockedAtUtc = null;
        user.MustSetPin = true;
        await _db.SaveChangesAsync(cancellationToken);
    }

    private Task<User> FindAsync(int userId, CancellationToken cancellationToken) =>
        _db.Users.FirstAsync(candidate => candidate.Id == userId, cancellationToken);
}
