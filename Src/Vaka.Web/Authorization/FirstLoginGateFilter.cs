using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Vaka.Web.Services;

namespace Vaka.Web.Authorization;

/// <summary>
/// Parolası değiştirilmemiş ya da PIN'i olmayan kullanıcıyı ilk giriş ekranına yönlendirir (BR-55).
/// Yetki politikası değil istek filtresidir; yetki denetiminden önce çalışır.
/// </summary>
public sealed class FirstLoginGateFilter : IAsyncAuthorizationFilter, IOrderedFilter
{
    private static readonly string[] ExemptPaths =
    [
        "/account/first-login",
        "/account/login",
        "/account/logout",
        "/account/access-denied",
        "/error"
    ];

    private readonly IAccountService _accounts;

    public FirstLoginGateFilter(IAccountService accounts)
    {
        _accounts = accounts;
    }

    public int Order => -1000;

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var httpContext = context.HttpContext;
        if (httpContext.User.Identity?.IsAuthenticated != true
            || !AccountPrincipal.TryGetUserId(httpContext.User, out var userId)
            || ExemptPaths.Any(path => httpContext.Request.Path.StartsWithSegments(path, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        var state = await _accounts.GetStateAsync(userId, httpContext.RequestAborted);
        if (state is { IsActive: true } && (state.MustChangePassword || state.MustSetPin))
        {
            context.Result = new RedirectToActionResult("FirstLogin", "Account", null);
        }
    }
}
