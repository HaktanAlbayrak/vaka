namespace Vaka.Web.Services;

public sealed record AccountState(
    int UserId,
    string FullName,
    string RegistryNo,
    string[] Roles,
    bool IsActive,
    bool MustChangePassword,
    bool MustSetPin);

public sealed record FirstLoginRequest(
    string? CurrentPassword,
    string? NewPassword,
    string? NewPin);

public interface IAccountService
{
    /// <summary>Sicil no ve parolayı doğrular; hatalı parola, bilinmeyen sicil no ve pasif hesap aynı şekilde boş döner.</summary>
    Task<AccountState?> SignInAsync(string registryNo, string password, CancellationToken cancellationToken);

    Task<AccountState?> GetStateAsync(int userId, CancellationToken cancellationToken);

    /// <summary>Hesabın bekleyen adımlarını tamamlar: parola değiştirilmemişse parolayı, PIN yoksa PIN'i (BR-55).</summary>
    Task<CommandResult> CompleteFirstLoginAsync(int userId, FirstLoginRequest request, CancellationToken cancellationToken);

    Task<CommandResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken cancellationToken);

    /// <summary>Eski PIN'i doğrular (yanlış PIN hatalı deneme sayılır, K-42); yeni PIN biçimini denetler ve yazar.</summary>
    Task<CommandResult> ChangePinAsync(int userId, string currentPin, string newPin, CancellationToken cancellationToken);
}
