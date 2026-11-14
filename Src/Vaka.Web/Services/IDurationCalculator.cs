namespace Vaka.Web.Services;

public interface IDurationCalculator
{
    /// <summary>
    /// Oturumun işlemsizlikten kapanıp kapanmadığı (NFR-09, PL-24, K12).
    /// İşlemsizlik = şimdi - son kullanıcı isteği anı
    /// Oturum kapanır: İşlemsizlik &gt;= OturumSüresi (dakika)
    /// </summary>
    /// <remarks>
    /// Son istek anı her kullanıcı isteğinde yenilenir; çerezin kayan süresi kullanılmaz. Saat geri giderse
    /// işlemsizlik negatif çıkar ve oturum kapanmaz.
    /// </remarks>
    bool IsSessionIdle(DateTimeOffset lastRequestAt, DateTimeOffset now, int sessionMinutes);
}
