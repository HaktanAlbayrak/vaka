using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaka.Web.Authorization;
using Vaka.Web.ViewModels.Home;

namespace Vaka.Web.Controllers;

[Authorize(Roles = Roles.Any)]
public sealed class HomeController : Controller
{
    [HttpGet("~/")]
    public IActionResult Index()
    {
        if (User.IsInRole(Roles.Operator))
        {
            return RedirectToAction("Index", "MyWork");
        }

        var roles = User.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToArray();
        if (roles.All(role => role == Roles.SuperAdmin))
        {
            return RedirectToAction("Index", "Users");
        }

        return View(new HomeViewModel
        {
            FullName = User.Identity?.Name ?? string.Empty,
            Roles = roles
        });
    }
}
