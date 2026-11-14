using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Vaka.Web.Services;

namespace Vaka.Web.Controllers;

public static class SessionControllerExtensions
{
    public static async Task<IActionResult> SignOutToLoginAsync(this Controller controller, string reason)
    {
        await controller.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return controller.RedirectToAction("Login", "Account", new { reason });
    }

    /// <summary>Alan adı verilen önekle birleşir; alana bağlı olmayan hata boş anahtarla eklenir.</summary>
    public static void AddErrors(this ModelStateDictionary modelState, CommandResult result, string prefix = "")
    {
        foreach (var error in result.Errors)
        {
            var key = error.Field.Length == 0 || prefix.Length == 0
                ? error.Field
                : $"{prefix}.{error.Field}";
            modelState.AddModelError(key, error.Message);
        }
    }
}
