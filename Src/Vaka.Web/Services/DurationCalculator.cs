namespace Vaka.Web.Services;

public sealed class DurationCalculator : IDurationCalculator
{
    /// <summary>
    /// Oturumun işlemsizlikten kapanıp kapanmadığı (NFR-09, PL-24, K12).
    /// İşlemsizlik = şimdi - son kullanıcı isteği anı
    /// Oturum kapanır: İşlemsizlik &gt;= OturumSüresi (dakika)
    /// </summary>
    public bool IsSessionIdle(DateTimeOffset lastRequestAt, DateTimeOffset now, int sessionMinutes) =>
        now - lastRequestAt >= TimeSpan.FromMinutes(sessionMinutes);
}
