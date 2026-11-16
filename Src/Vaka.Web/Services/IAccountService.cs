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
    /// <summary>
    /// Sicil no ve parolayı doğrular. Bilinmeyen sicil no, pasif hesap ve yanlış parola aynı <see cref="SignInStatus.Invalid"/> sonucunu verir.
    /// Yanlış parola giriş sayacına yazılır; kilitli hesapta doğru parola da <see cref="SignInStatus.LockedUntil"/> alır.
    /// </summary>
    Task<SignInResult> SignInAsync(string registryNo, string password, CancellationToken cancellationToken);

    /// <summary>
    /// Sicil no ve PIN'i doğrular. Yalnız tüm rolleri Operatör olan hesap uygundur; uygun olmayan hesap, bilinmeyen sicil no ve pasif hesap
    /// PIN doğrulanmadan ve hiçbir sayaç yazılmadan <see cref="SignInStatus.Invalid"/> alır. Uygun ama ilk girişini tamamlamamış hesap
    /// (parola değişmemiş, PIN yok ya da sıfırlanmış) <see cref="SignInStatus.PasswordRequired"/> alır (BR-55).
    /// İşlem PIN'i kilitli hesap PIN doğrulanmadan <see cref="SignInStatus.PinLocked"/> alır; parola girişi açık kalır.
    /// Sayaç giriş sayacıdır, işlem PIN sayacına dokunulmaz.
    /// </summary>
    Task<SignInResult> SignInWithPinAsync(string registryNo, string pin, CancellationToken cancellationToken);

    Task<AccountState?> GetStateAsync(int userId, CancellationToken cancellationToken);

    /// <summary>Hesabın bekleyen adımlarını tamamlar: parola değiştirilmemişse parolayı, PIN yoksa PIN'i (BR-55).</summary>
    Task<CommandResult> CompleteFirstLoginAsync(int userId, FirstLoginRequest request, CancellationToken cancellationToken);

    Task<CommandResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken cancellationToken);

    /// <summary>Eski PIN'i doğrular (yanlış PIN hatalı deneme sayılır, K-42); yeni PIN biçimini denetler ve yazar.</summary>
    Task<CommandResult> ChangePinAsync(int userId, string currentPin, string newPin, CancellationToken cancellationToken);
}
