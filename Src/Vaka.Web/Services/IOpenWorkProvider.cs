using Vaka.Web.ViewModels.Users;

namespace Vaka.Web.Services;

public interface IOpenWorkProvider
{
    /// <summary>
    /// Kullanıcının pasifleştirmeyi engelleyen açık işleri (BR-36). <c>Url</c> yalnız görüntüleyenin erişebildiği ekrana verilir (PL-16).
    /// </summary>
    Task<OpenWorkItemViewModel[]> ListAsync(int targetUserId, int viewerUserId, CancellationToken cancellationToken);
}
