using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaka.Web.Authorization;
using Vaka.Web.Services;
using Vaka.Web.ViewModels.Account;

namespace Vaka.Web.Controllers;

[Authorize(Roles = Roles.Any)]
[Route("[controller]/[action]")]
public sealed class AccountController : Controller
{
    private const string StatusKey = "StatusMessage";
    private const string ModeCookie = "vaka-login-mode";

    private readonly IAccountService _accounts;
    private readonly ISystemSettingsReader _settings;
    private readonly TimeProvider _time;

    public AccountController(IAccountService accounts, ISystemSettingsReader settings, TimeProvider time)
    {
        _accounts = accounts;
        _settings = settings;
        _time = time;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl, string? reason, LoginMode? mode)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel
        {
            Mode = mode is { } requested && Enum.IsDefined(requested) ? requested : ReadModeCookie(),
            ReturnUrl = returnUrl,
            StatusMessage = SessionEndReason.Message(reason)
        });
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
    {
        var isPin = model.Mode == LoginMode.Pin;
        ModelState.Remove(nameof(LoginViewModel.Mode));
        if (isPin && string.IsNullOrEmpty(model.Pin))
        {
            ModelState.AddModelError(nameof(LoginViewModel.Pin), "PIN girin.");
        }
        else if (!isPin && string.IsNullOrEmpty(model.Password))
        {
            ModelState.AddModelError(nameof(LoginViewModel.Password), "Parola girin.");
        }

        if (!ModelState.IsValid)
        {
            return View(ClearSecrets(model));
        }

        var registryNo = model.RegistryNo.Trim();
        var result = isPin
            ? await _accounts.SignInWithPinAsync(registryNo, model.Pin!, cancellationToken)
            : await _accounts.SignInAsync(registryNo, model.Password, cancellationToken);

        if (result.Status != SignInStatus.Success)
        {
            ClearSecrets(model);
            model.LockedUntilLocal = result.LockedUntilLocal;
            model.ErrorMessage = result.Status switch
            {
                SignInStatus.PinLocked => "PIN'iniz kilitli. Parola ile girin; PIN'i yöneticiniz sıfırlar.",
                SignInStatus.PasswordRequired => "İlk girişinizi parola ile yapın.",
                SignInStatus.Invalid => isPin ? "Sicil no ya da PIN hatalı." : "Sicil no ya da parola hatalı.",
                _ => null
            };
            return View(model);
        }

        WriteModeCookie(model.Mode);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            AccountPrincipal.Create(result.State!, CookieAuthenticationDefaults.AuthenticationScheme, AccountPrincipal.NewSessionId()),
            VakaCookieEvents.CreateProperties(_time.GetUtcNow()));

        return !string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)
            ? LocalRedirect(model.ReturnUrl)
            : RedirectToAction("Index", "Home");
    }

    // Oturumu düşmüş kişi de "Kullanıcı değiştir"e basabilmeli (K11a); token oturum kimliğine bağlı olduğundan doğrulanamaz.
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    [HttpPost]
    public Task<IActionResult> Logout() => this.SignOutToLoginAsync(SessionEndReason.SignedOut);

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccessDenied()
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> FirstLogin(CancellationToken cancellationToken)
    {
        var state = await _accounts.GetStateAsync(User.GetUserId(), cancellationToken);
        if (state is null || (!state.MustChangePassword && !state.MustSetPin))
        {
            return RedirectToAction("Index", "Home");
        }

        return View(await BuildFirstLoginAsync(state, new FirstLoginViewModel(), cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> FirstLogin(FirstLoginViewModel model, CancellationToken cancellationToken)
    {
        var state = await _accounts.GetStateAsync(User.GetUserId(), cancellationToken);
        if (state is null || (!state.MustChangePassword && !state.MustSetPin))
        {
            return RedirectToAction("Index", "Home");
        }

        if (ModelState.IsValid)
        {
            var request = new FirstLoginRequest(
                state.MustChangePassword ? model.CurrentPassword : null,
                state.MustChangePassword ? model.NewPassword : null,
                state.MustSetPin ? model.NewPin : null);
            var result = await _accounts.CompleteFirstLoginAsync(state.UserId, request, cancellationToken);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddErrors(result);
        }

        return View(await BuildFirstLoginAsync(state, model, cancellationToken));
    }

    [HttpGet]
    public async Task<IActionResult> Security(CancellationToken cancellationToken)
    {
        var model = await BuildSecurityAsync(new ChangePasswordViewModel(), new ChangePinViewModel(), cancellationToken);
        model.StatusMessage = TempData[StatusKey] as string;
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword([Bind(Prefix = "Password")] ChangePasswordViewModel model, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var result = await _accounts.ChangePasswordAsync(User.GetUserId(), model.CurrentPassword, model.NewPassword, cancellationToken);
            if (result.Succeeded)
            {
                TempData[StatusKey] = "Parola değiştirildi.";
                return RedirectToAction(nameof(Security));
            }

            ModelState.AddErrors(result, "Password");
        }

        return View(nameof(Security), await BuildSecurityAsync(new ChangePasswordViewModel(), new ChangePinViewModel(), cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> ChangePin([Bind(Prefix = "Pin")] ChangePinViewModel model, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var result = await _accounts.ChangePinAsync(User.GetUserId(), model.CurrentPin, model.NewPin, cancellationToken);
            if (result.Succeeded)
            {
                TempData[StatusKey] = "PIN değiştirildi.";
                return RedirectToAction(nameof(Security));
            }

            if (result.Status == CommandStatus.PinLocked)
            {
                return await this.SignOutToLoginAsync(SessionEndReason.PinLocked);
            }

            ModelState.AddErrors(result, "Pin");
        }

        return View(nameof(Security), await BuildSecurityAsync(new ChangePasswordViewModel(), new ChangePinViewModel(), cancellationToken));
    }

    private static LoginViewModel ClearSecrets(LoginViewModel model)
    {
        model.Password = string.Empty;
        model.Pin = null;
        return model;
    }

    private LoginMode ReadModeCookie() =>
        Request.Cookies[ModeCookie] == "password" ? LoginMode.Password : LoginMode.Pin;

    private void WriteModeCookie(LoginMode mode) =>
        Response.Cookies.Append(ModeCookie, mode == LoginMode.Password ? "password" : "pin", new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Secure = Request.IsHttps,
            IsEssential = true,
            MaxAge = TimeSpan.FromDays(365)
        });

    private async Task<FirstLoginViewModel> BuildFirstLoginAsync(AccountState state, FirstLoginViewModel posted, CancellationToken cancellationToken)
    {
        posted.FullName = state.FullName;
        posted.MustChangePassword = state.MustChangePassword;
        posted.MustSetPin = state.MustSetPin;
        posted.PinLength = await _settings.GetValueAsync(SettingKeys.PinLength, cancellationToken);
        return posted;
    }

    private async Task<SecurityViewModel> BuildSecurityAsync(ChangePasswordViewModel password, ChangePinViewModel pin, CancellationToken cancellationToken) =>
        new()
        {
            PinLength = await _settings.GetValueAsync(SettingKeys.PinLength, cancellationToken),
            Password = password,
            Pin = pin
        };
}
