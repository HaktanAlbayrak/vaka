using Vaka.Web.ViewModels.Users;

namespace Vaka.Web.Services;

public interface IUserDirectory
{
    Task<UserListViewModel> ListAsync(int actorId, string? query, UserStatusFilter status, int page, CancellationToken cancellationToken);

    /// <summary><paramref name="targetId"/> boşsa yeni kullanıcı formu döner; hedef işlemi yapanın kapsamı dışındaysa <see cref="CommandStatus.Forbidden"/>.</summary>
    Task<LookupResult<UserFormViewModel>> GetFormAsync(int actorId, int? targetId, CancellationToken cancellationToken);

    Task<LookupResult<UserDeactivateViewModel>> GetDeactivateAsync(int actorId, int targetId, CancellationToken cancellationToken);
}
