using Vaka.Web.Authorization;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data.Seed;

public static class AuthSeed
{
    // PasswordHasher<string> V3 özeti. Düz parola depoda tutulmaz; ilk girişte değişir.
    private const string SuperAdminPasswordHash =
        "AQAAAAIAAYagAAAAEP7pSGzAxhrALT7qPx0axoziu+k8Bm98QYmCO4lubR57uLg3035M3nigzCHOMojTwA==";

    public static readonly User SuperAdminUser = new()
    {
        Id = 1,
        RegistryNo = "0001",
        FullName = "Süper Admin",
        PasswordHash = SuperAdminPasswordHash,
        PinHash = null,
        MustChangePassword = true,
        MustSetPin = true,
        PinFailedCount = 0,
        PinLockedAtUtc = null,
        IsActive = true,
        CreatedAtUtc = new DateTime(2026, 11, 14, 0, 0, 0, DateTimeKind.Utc)
    };

    public static readonly UserRole SuperAdminRole = new()
    {
        UserId = 1,
        Role = Roles.SuperAdmin
    };
}
