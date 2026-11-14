using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Vaka.Web.Controllers;

[AllowAnonymous]
[Route("[controller]/[action]")]
public sealed class PrototypeController : Controller
{
    private readonly IWebHostEnvironment _environment;

    public PrototypeController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!_environment.IsDevelopment())
        {
            context.Result = NotFound();
        }
    }

    [HttpGet]
    public IActionResult MyWork() => View();

    [HttpGet]
    public IActionResult Run() => View();

    [HttpGet]
    public IActionResult NewRecord() => View();

    [HttpGet]
    public IActionResult Records() => View();

    [HttpGet]
    public IActionResult Dashboard() => View();
}
