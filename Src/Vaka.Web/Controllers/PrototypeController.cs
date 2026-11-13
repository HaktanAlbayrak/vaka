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

    public IActionResult MyWork() => View();

    public IActionResult Run() => View();

    public IActionResult NewRecord() => View();

    public IActionResult Records() => View();

    public IActionResult Dashboard() => View();
}
