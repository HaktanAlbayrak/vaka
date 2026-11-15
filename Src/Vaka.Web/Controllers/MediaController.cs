using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaka.Web.Authorization;
using Vaka.Web.Services;

namespace Vaka.Web.Controllers;

[Authorize(Roles = Roles.RecordReaders)]
[Route("media")]
public sealed class MediaController : Controller
{
    private readonly IStepMediaService _media;
    private readonly IMediaStore _store;

    public MediaController(IStepMediaService media, IMediaStore store)
    {
        _media = media;
        _store = store;
    }

    // Sahada bağlantı zorunlu olduğu için medya önbelleğe alınmaz (Brief §9.6); içerik adresli olsa da yetki her istekte denetlenir.
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Show(int id, CancellationToken cancellationToken)
    {
        var lookup = await _media.FindAsync(id, User.IsInRole(Roles.Manager), cancellationToken);
        if (lookup.Status != CommandStatus.Succeeded)
        {
            return NotFound();
        }

        var file = lookup.Value!;
        var stream = _store.Open(file.Sha256);
        if (stream is null)
        {
            return NotFound();
        }

        Response.Headers.CacheControl = "private, no-store";
        Response.Headers.XContentTypeOptions = "nosniff";
        return File(stream, file.ContentType, enableRangeProcessing: true);
    }
}
