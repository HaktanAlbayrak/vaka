using System.Globalization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.WebUtilities;
using Vaka.Web.Services;

namespace Vaka.Web.Authorization;

public sealed class VakaCookieEvents : CookieAuthenticationEvents
{
    public const string LastRequestKey = "Vaka.LastRequestAt";

    private readonly ISystemSettingsReader _settings;
    private readonly IAccountService _accounts;
    private readonly IDurationCalculator _durations;
    private readonly TimeProvider _time;

    public VakaCookieEvents(
        ISystemSettingsReader settings,
        IAccountService accounts,
        IDurationCalculator durations,
        TimeProvider time)
    {
        _settings = settings;
        _accounts = accounts;
        _durations = durations;
        _time = time;
    }

    public static AuthenticationProperties CreateProperties(DateTimeOffset now) =>
        new() { Items = { [LastRequestKey] = now.ToString("O", CultureInfo.InvariantCulture) } };

    /// <summary>
    /// Oturum son kullanıcı isteğinden sonraki işlemsizlikle kapanır (NFR-09, PL-24).
    /// İşlemsizlik = şimdi - son kullanıcı isteği anı; oturum kapanır: İşlemsizlik &gt;= OturumSüresi (dakika), karar <see cref="IDurationCalculator.IsSessionIdle"/>.
    /// Çerezin kayan süresi kullanılmaz; her doğrulanan istek son istek anını yeniler. Hesap pasifleşmişse oturum düşer,
    /// rol ya da ad değişmişse kimlik veritabanındaki değerle yenilenir.
    /// </summary>
    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        var cancellationToken = context.HttpContext.RequestAborted;
        var now = _time.GetUtcNow();

        if (!TryReadLastRequest(context.Properties, out var lastRequestAt)
            || !AccountPrincipal.TryGetUserId(context.Principal!, out var userId))
        {
            await EndSessionAsync(context, SessionEndReason.Expired);
            return;
        }

        var sessionMinutes = await _settings.GetValueAsync(SettingKeys.SessionMinutes, cancellationToken);
        if (_durations.IsSessionIdle(lastRequestAt, now, sessionMinutes))
        {
            await EndSessionAsync(context, SessionEndReason.Expired);
            return;
        }

        var state = await _accounts.GetStateAsync(userId, cancellationToken);
        if (state is null || !state.IsActive)
        {
            await EndSessionAsync(context, SessionEndReason.Inactive);
            return;
        }

        if (IdentityChanged(context, state))
        {
            context.ReplacePrincipal(AccountPrincipal.Create(state, context.Scheme.Name));
        }

        context.Properties.Items[LastRequestKey] = now.ToString("O", CultureInfo.InvariantCulture);
        context.ShouldRenew = true;
    }

    public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
    {
        var redirectUri = context.RedirectUri;
        if (context.HttpContext.Items[SessionEndReason.ItemKey] is string reason)
        {
            redirectUri = QueryHelpers.AddQueryString(redirectUri, SessionEndReason.QueryKey, reason);
        }

        context.Response.Redirect(redirectUri);
        return Task.CompletedTask;
    }

    private static bool TryReadLastRequest(AuthenticationProperties properties, out DateTimeOffset lastRequestAt)
    {
        lastRequestAt = default;
        return properties.Items.TryGetValue(LastRequestKey, out var raw)
            && DateTimeOffset.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out lastRequestAt);
    }

    private static bool IdentityChanged(CookieValidatePrincipalContext context, AccountState state)
    {
        var cookieRoles = context.Principal!.FindAll(System.Security.Claims.ClaimTypes.Role).Select(claim => claim.Value);
        var sameRoles = new HashSet<string>(cookieRoles).SetEquals(state.Roles);
        return !sameRoles || context.Principal.Identity?.Name != state.FullName;
    }

    private static async Task EndSessionAsync(CookieValidatePrincipalContext context, string reason)
    {
        context.RejectPrincipal();
        context.HttpContext.Items[SessionEndReason.ItemKey] = reason;
        await context.HttpContext.SignOutAsync(context.Scheme.Name);
    }
}
