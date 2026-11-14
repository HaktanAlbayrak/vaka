using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Services;

public interface IMachineBlockProvider
{
    /// <summary>Makinenin kullanımdan kaldırılmasını engelleyen aktif ya da beklemedeki kayıtlar (BR-35, K-25). <c>Url</c> yalnız görüntüleyenin erişebildiği ekrana verilir.</summary>
    Task<BlockingRecordViewModel[]> ListAsync(int machineId, int viewerUserId, CancellationToken cancellationToken);
}
