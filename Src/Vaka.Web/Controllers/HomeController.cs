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

        if (User.IsInRole(Roles.Manager) || User.IsInRole(Roles.Auditor))
        {
            return RedirectToAction("Index", "Records");
        }

        var roles = User.GetRoles();
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
