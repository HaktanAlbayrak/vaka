using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;

namespace Vaka.Web.Services;

public sealed class SystemSettingsReader : ISystemSettingsReader
{
    private readonly VakaDbContext _db;

    public SystemSettingsReader(VakaDbContext db)
    {
        _db = db;
    }

    public async Task<int> GetValueAsync(string key, CancellationToken cancellationToken)
    {
        var value = await _db.SystemSettings
            .AsNoTracking()
            .Where(setting => setting.Key == key)
            .Select(setting => (int?)setting.Value)
            .FirstOrDefaultAsync(cancellationToken);

        return value ?? throw new InvalidOperationException($"Sistem ayarı tanımlı değil: {key}.");
    }
}
