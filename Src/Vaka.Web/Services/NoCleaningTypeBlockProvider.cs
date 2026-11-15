using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Services;

public sealed class NoCleaningTypeBlockProvider : ICleaningTypeBlockProvider
{
    public Task<BlockingRecordViewModel[]> ListAsync(int cleaningTypeId, int viewerUserId, CancellationToken cancellationToken) =>
        Task.FromResult<BlockingRecordViewModel[]>([]);
}
