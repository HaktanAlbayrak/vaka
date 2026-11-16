namespace Vaka.Web.Services;

/// <summary>
/// Parola ve PIN girişinin ortak hatalı deneme sayacı ve süreli kilidi. Sayaç işlem PIN sayacından (<c>PinFailedCount</c>) ayrıdır.
/// Sayaç artışı tek atomik SQL güncellemesidir; kilit ayrı ve koşullu (sayaç sınırda ve kilit boş) bir güncellemeyle yazılır. Eşzamanlı denemelerde artış kaybolmaz, kilidi bir deneme yazar ve ilk yazılan bitiş korunur.
/// </summary>
public interface ILoginAttemptGuard
{
    /// <summary>
    /// Giriş açıksa boş, kapalıysa kilit bitişini (UTC) döner. Kilit açık = <c>LoginLockedUntilUtc &gt; şimdi</c>.
    /// Süresi dolmuş kilit (<c>LoginLockedUntilUtc &lt;= şimdi</c>) burada temizlenir ve sayaç sıfırdan başlar.
    /// </summary>
    Task<DateTime?> GetLockedUntilAsync(int userId, CancellationToken cancellationToken);

    /// <summary>
    /// Hatalı denemeyi sayar. Sayaç <c>PinAttemptLimit</c> değerine ulaşınca kilit yazılır.
    /// Kilit bitişi = sınıra ulaşan hatalı denemenin sunucu anı + <c>LoginLockMinutes</c> dakika.
    /// Bu deneme girişi kilitlediyse bitişi (UTC) döner, aksi halde boş.
    /// </summary>
    Task<DateTime?> RegisterFailureAsync(int userId, CancellationToken cancellationToken);

    /// <summary>Başarılı girişte ve yönetici kilit açmasında sayacı sıfırlar, kilidi kaldırır.</summary>
    Task ResetAsync(int userId, CancellationToken cancellationToken);
}
