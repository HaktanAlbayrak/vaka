namespace Vaka.Web.Services;

public enum SignInStatus
{
    Success,
    Invalid,
    LockedUntil,
    PasswordRequired,
    PinLocked
}

/// <param name="LockedUntilLocal">Yalnız <see cref="SignInStatus.LockedUntil"/> durumunda dolu; Türkiye saatiyle kilit bitişi.</param>
/// <param name="State">Yalnız <see cref="SignInStatus.Success"/> durumunda dolu.</param>
public sealed record SignInResult(SignInStatus Status, DateTime? LockedUntilLocal = null, AccountState? State = null);
