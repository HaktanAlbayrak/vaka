using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaka.Web.Authorization;
using Vaka.Web.Services;

namespace Vaka.Web.Controllers;

[Authorize(Roles = Roles.RecordReaders)]
public sealed class MachineBoardController : Controller
{
    private readonly IMachineBoardReader _board;

    public MachineBoardController(IMachineBoardReader board)
    {
        _board = board;
    }

    [HttpGet("~/machine-board")]
    public async Task<IActionResult> Index(int? facilityId, CancellationToken cancellationToken) =>
        View(await _board.GetAsync(facilityId, User.GetRoles(), cancellationToken));
}
