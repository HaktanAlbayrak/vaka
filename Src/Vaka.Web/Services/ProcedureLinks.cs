using Microsoft.AspNetCore.Routing;

namespace Vaka.Web.Services;

public sealed class ProcedureLinks : IProcedureLinks
{
    private readonly LinkGenerator _links;

    public ProcedureLinks(LinkGenerator links)
    {
        _links = links;
    }

    public string Version(int versionId, ProcedureVersionStatus status) =>
        _links.GetPathByAction(
            status == ProcedureVersionStatus.Draft ? "Edit" : "Details",
            "ProcedureVersions",
            new { id = versionId }) ?? throw new InvalidOperationException("Versiyon adresi üretilemedi.");

    public string Media(int mediaId) =>
        _links.GetPathByAction("Show", "Media", new { id = mediaId }) ?? throw new InvalidOperationException("Medya adresi üretilemedi.");
}
