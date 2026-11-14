using Vaka.Web.ViewModels.Users;

namespace Vaka.Web.Services;

public sealed class NoOpenWorkProvider : IOpenWorkProvider
{
    public Task<OpenWorkItemViewModel[]> ListAsync(int targetUserId, int viewerUserId, CancellationToken cancellationToken) =>
        Task.FromResult<OpenWorkItemViewModel[]>([]);
}
