using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaka.Web.Authorization;
using Vaka.Web.Services;

namespace Vaka.Web.Controllers;

[Authorize(Roles = Roles.ManagerOrAuditor)]
public sealed class DashboardController : Controller
{
    private readonly IDashboardReader _dashboard;

    public DashboardController(IDashboardReader dashboard)
    {
        _dashboard = dashboard;
    }

    [HttpGet("~/dashboard")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await _dashboard.GetAsync(cancellationToken));
}
