using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaka.Web.Authorization;
using Vaka.Web.Services;
using Vaka.Web.ViewModels.Breaks;
using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Controllers;

[Authorize(Roles = Roles.Operator)]
[Route("[controller]/[action]")]
public sealed class BreaksController : Controller
{
    private readonly IBreakService _breakService;
    private readonly IBreakReader _breaks;
    private readonly IRecordRunReader _run;

    public BreaksController(IBreakService breakService, IBreakReader breaks, IRecordRunReader run)
    {
        _breakService = breakService;
        _breaks = breaks;
        _run = run;
    }

    [HttpGet]
    public async Task<IActionResult> Current(CancellationToken cancellationToken)
    {
        var current = await _breaks.GetCurrentAsync(User.GetUserId(), cancellationToken);
        return current is null ? Redirect("/my-work") : View(current);
    }

    [HttpPost("{recordId:int}")]
    public async Task<IActionResult> Start(int recordId, StartBreakViewModel model, CancellationToken cancellationToken)
    {
        var result = await _breakService.StartAsync(recordId, User.GetUserId(), new StartBreakRequest(model.CommandId, model.Pin), cancellationToken);
        return result.Status switch
        {
            CommandStatus.Succeeded => RedirectToAction(nameof(Current)),
            CommandStatus.NotFound => NotFound(),
            CommandStatus.Forbidden => StatusCode(StatusCodes.Status403Forbidden),
            CommandStatus.OnBreak => RedirectToAction(nameof(Current)),
            _ => await RenderRunErrorAsync(recordId, result, model.CommandId, cancellationToken)
        };
    }

    [HttpPost("{breakId:int}")]
    public async Task<IActionResult> End(int breakId, EndBreakViewModel model, CancellationToken cancellationToken)
    {
        var result = await _breakService.EndAsync(breakId, User.GetUserId(), new EndBreakRequest(model.CommandId, model.Pin), cancellationToken);
        switch (result.Status)
        {
            case CommandStatus.Succeeded:
                return RedirectToAction("Run", "Records", new { id = result.EntityId });
            case CommandStatus.NotFound:
                return NotFound();
            case CommandStatus.Forbidden:
                return StatusCode(StatusCodes.Status403Forbidden);
        }

        var current = await _breaks.GetCurrentAsync(User.GetUserId(), cancellationToken);
        if (current is null)
        {
            return Redirect("/my-work");
        }

        current.CommandId = model.CommandId == Guid.Empty ? current.CommandId : model.CommandId;
        ModelState.Clear();
        ModelState.AddErrors(result);
        current.ErrorMessage = string.Join(" ", result.Errors.Select(error => error.Message));
        return View("Current", current);
    }

    private async Task<IActionResult> RenderRunErrorAsync(int recordId, CommandResult result, Guid commandId, CancellationToken cancellationToken)
    {
        var lookup = await _run.GetAsync(recordId, User.GetUserId(), new RunViewOptions(), cancellationToken);
        if (lookup.Value is not { } model)
        {
            return NotFound();
        }

        ModelState.Clear();
        ModelState.AddErrors(result);
        model.CommandId = commandId == Guid.Empty ? model.CommandId : commandId;
        model.ErrorMessage = string.Join(" ", result.Errors.Select(error => error.Message));
        return View("~/Views/Records/Run.cshtml", model);
    }
}
