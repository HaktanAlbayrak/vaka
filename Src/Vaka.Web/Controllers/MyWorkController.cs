using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaka.Web.Authorization;
using Vaka.Web.Services;

namespace Vaka.Web.Controllers;

[Authorize(Roles = Roles.Operator)]
public sealed class MyWorkController : Controller
{
    private readonly IMyWorkReader _myWork;

    public MyWorkController(IMyWorkReader myWork)
    {
        _myWork = myWork;
    }

    [HttpGet("~/my-work")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await _myWork.GetAsync(User.GetUserId(), cancellationToken));
}
