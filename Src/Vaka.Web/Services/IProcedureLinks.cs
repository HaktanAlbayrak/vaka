namespace Vaka.Web.Services;

public interface IProcedureLinks
{
    /// <summary>Taslak düzenleyiciye, yayınlanmış versiyon görüntülemeye giden adres.</summary>
    string Version(int versionId, ProcedureVersionStatus status);

    string Media(int mediaId);
}
