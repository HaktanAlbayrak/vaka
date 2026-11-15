using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Services;

public interface ICleaningTypeBlockProvider
{
    /// <summary>Tipin pasifleştirilmesini engelleyen aktif ya da beklemedeki kayıtlar (BR-35). <c>Url</c> yalnız görüntüleyenin erişebildiği ekrana verilir.</summary>
    Task<BlockingRecordViewModel[]> ListAsync(int cleaningTypeId, int viewerUserId, CancellationToken cancellationToken);
}
