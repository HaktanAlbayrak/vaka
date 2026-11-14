namespace Vaka.Web.Services;

public sealed record UserCreateRequest(string? RegistryNo, string? FullName, string[] Roles, string? TemporaryPassword);

public sealed record UserUpdateRequest(string? FullName, string[] Roles);

/// <summary>
/// Her metot önce işlemi yapanın ve hedefin rollerini veritabanından okuyup kapsamı (SK-9) denetler; kapsam dışı
/// <see cref="CommandStatus.Forbidden"/> döner ve hiçbir şey değişmez. PIN isteyen işlemlerde PIN en son, diğer iş kuralları geçtikten sonra doğrulanır.
/// PIN reddi <c>CommandResult.FromPin(status, "Pin")</c> ile alan adı <c>Pin</c> olarak döner; controller öneki ekler (<c>ResetPin.Pin</c>, <c>Activate.Pin</c>).
/// </summary>
public interface IUserAdministration
{
    /// <summary>Başarıda <see cref="CommandResult.EntityId"/> yeni kullanıcı kimliğidir.</summary>
    Task<CommandResult> CreateAsync(int actorId, UserCreateRequest request, CancellationToken cancellationToken);

    /// <summary>Sicil no değişmez (PL-27); kendi hesabında roller değişmez (PL-16).</summary>
    Task<CommandResult> UpdateAsync(int actorId, int targetId, UserUpdateRequest request, CancellationToken cancellationToken);

    Task<CommandResult> DeactivateAsync(int actorId, int targetId, string? pin, CancellationToken cancellationToken);

    Task<CommandResult> ActivateAsync(int actorId, int targetId, string? pin, CancellationToken cancellationToken);

    Task<CommandResult> ResetPinAsync(int actorId, int targetId, string? pin, CancellationToken cancellationToken);

    Task<CommandResult> ResetPasswordAsync(int actorId, int targetId, string? temporaryPassword, string? pin, CancellationToken cancellationToken);
}
