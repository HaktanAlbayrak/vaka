using System.Globalization;
using System.Security.Claims;
using Vaka.Web.Services;

namespace Vaka.Web.Authorization;

public static class AccountPrincipal
{
    public static ClaimsPrincipal Create(AccountState state, string authenticationType, string sessionId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, state.FullName),
            new(ClaimTypes.NameIdentifier, state.UserId.ToString(CultureInfo.InvariantCulture)),
            new(VakaClaimTypes.UserId, state.UserId.ToString(CultureInfo.InvariantCulture)),
            new(VakaClaimTypes.RegistryNo, state.RegistryNo),
            new(VakaClaimTypes.SessionId, sessionId)
        };
        claims.AddRange(state.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType, ClaimTypes.Name, ClaimTypes.Role));
    }

    public static string[] GetRoles(this ClaimsPrincipal principal) =>
        [.. principal.FindAll(ClaimTypes.Role).Select(claim => claim.Value)];

    /// <summary>Her girişte rastgele üretilen oturum kimliği (K23); çıkışta iptal listesine girer.</summary>
    public static string NewSessionId() => Convert.ToHexStringLower(System.Security.Cryptography.RandomNumberGenerator.GetBytes(16));

    public static bool TryGetUserId(ClaimsPrincipal principal, out int userId) =>
        int.TryParse(principal.FindFirstValue(VakaClaimTypes.UserId), CultureInfo.InvariantCulture, out userId);

    public static int GetUserId(this ClaimsPrincipal principal) =>
        TryGetUserId(principal, out var userId)
            ? userId
            : throw new InvalidOperationException("Oturum kimliği taşımayan istek.");
}
