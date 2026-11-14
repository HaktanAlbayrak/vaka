namespace Vaka.Web.Services;

public enum PinCheckStatus
{
    Accepted,
    Wrong,
    LockedNow,
    Locked,
    NotSet
}

public interface IPinService
{
    /// <summary>
    /// PIN'i özetle karşılaştırır; uzunluk sınırı koymaz. Hatalı denemede sayacı artırır ve <c>PinDenemeSınırı</c>
    /// değerine ulaşan denemede kilitler (<see cref="PinCheckStatus.LockedNow"/>); doğru PIN sayacı sıfırlar.
    /// Kilitliyken PIN doğru olsa da <see cref="PinCheckStatus.Locked"/> döner ve sayaç değişmez. PIN günlüğe yazılmaz.
    /// </summary>
    Task<PinCheckStatus> VerifyAsync(int userId, string? pin, CancellationToken cancellationToken);

    /// <summary>Yeni PIN'in özetini yazar; sayacı, kilidi ve PIN belirleme bayrağını temizler. Biçim denetimi çağıranda yapılır.</summary>
    Task SetAsync(int userId, string newPin, CancellationToken cancellationToken);

    /// <summary>PIN'i, sayacı ve kilidi temizler, PIN belirleme bayrağını açar.</summary>
    Task ResetAsync(int userId, CancellationToken cancellationToken);
}
