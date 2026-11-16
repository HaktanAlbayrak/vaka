using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaka.Web.Authorization;
using Vaka.Web.Services;
using Vaka.Web.ViewModels.Reports;

namespace Vaka.Web.Controllers;

[Authorize(Roles = Roles.ManagerOrAuditor)]
[Route("[controller]/[action]")]
public sealed class ReportsController : Controller
{
    private readonly IDurationReportReader _durations;
    private readonly IMaterialTraceReader _materials;
    private readonly IMachineReportReader _machines;

    public ReportsController(IDurationReportReader durations, IMaterialTraceReader materials, IMachineReportReader machines)
    {
        _durations = durations;
        _materials = materials;
        _machines = machines;
    }

    [HttpGet]
    public async Task<IActionResult> Durations(DurationReportFilterViewModel filter, CancellationToken cancellationToken)
    {
        var invalid = !ModelState.IsValid;
        ModelState.Clear();
        return View(await _durations.BuildAsync(filter, invalid ? ReportMessages.InvalidFilter : null, cancellationToken));
    }

    [HttpGet]
    public async Task<IActionResult> Machine(MachineReportFilterViewModel filter, CancellationToken cancellationToken)
    {
        var invalid = !ModelState.IsValid;
        ModelState.Clear();
        return View(await _machines.BuildAsync(filter, invalid ? ReportMessages.InvalidFilter : null, cancellationToken));
    }

    [HttpGet]
    public async Task<IActionResult> Materials(MaterialTraceViewModel request, CancellationToken cancellationToken)
    {
        var invalid = !ModelState.IsValid;
        ModelState.Clear();
        return View(await _materials.BuildAsync(request, invalid ? ReportMessages.InvalidFilter : null, cancellationToken));
    }
}
