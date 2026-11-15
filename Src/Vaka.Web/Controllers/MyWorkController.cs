using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaka.Web.Authorization;
using Vaka.Web.Services;

namespace Vaka.Web.Controllers;

[Authorize(Roles = Roles.Operator)]
public sealed class MyWorkController : Controller
{
    private readonly IMyWorkReader _myWork;
    private readonly IBreakReader _breaks;

    public MyWorkController(IMyWorkReader myWork, IBreakReader breaks)
    {
        _myWork = myWork;
        _breaks = breaks;
    }

    [HttpGet("~/my-work")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (await _breaks.IsOnBreakAsync(User.GetUserId(), cancellationToken))
        {
            return RedirectToAction("Current", "Breaks");
        }

        return View(await _myWork.GetAsync(User.GetUserId(), cancellationToken));
    }
}
