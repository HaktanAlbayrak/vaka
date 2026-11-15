using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Vaka.Web.Controllers;

[AllowAnonymous]
[IgnoreAntiforgeryToken]
[NoFormBinding]
[Route("error")]
public sealed class ErrorController : Controller
{
    [Route("")]
    public IActionResult Failure()
    {
        Response.StatusCode = StatusCodes.Status500InternalServerError;
        return View(nameof(Failure));
    }

    [Route("{code:int}")]
    public IActionResult Status([FromRoute] int code)
    {
        if (code is < 400 or > 599)
        {
            code = StatusCodes.Status404NotFound;
        }

        Response.StatusCode = code;
        return code switch
        {
            StatusCodes.Status403Forbidden => View("Forbidden"),
            StatusCodes.Status404NotFound => View("NotFound"),
            _ => View(nameof(Failure))
        };
    }
}
