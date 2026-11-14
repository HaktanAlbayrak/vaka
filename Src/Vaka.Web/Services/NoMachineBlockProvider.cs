using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Services;

public sealed class NoMachineBlockProvider : IMachineBlockProvider
{
    public Task<BlockingRecordViewModel[]> ListAsync(int machineId, int viewerUserId, CancellationToken cancellationToken) =>
        Task.FromResult<BlockingRecordViewModel[]>([]);
}
