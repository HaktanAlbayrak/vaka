namespace Vaka.Web.Services;

public interface IWriteTransactionRunner
{
    /// <summary>
    /// <paramref name="action"/> yazma kilidini baştan alan tek işlemde çalışır (SQLite, Serializable yalıtım = <c>BEGIN IMMEDIATE</c>);
    /// kontrol ile yazma arasına başka yazar giremez (BR-23, BR-37). İzleyici kilitten sonra temizlenir, durum veritabanından yeniden okunur.
    /// Sonuç başarılıysa ya da PIN reddiyse (sayaç yazılmıştır) işlem onaylanır; diğer reddlerde yazılanlar geri alınır (F1-A2).
    /// Kilit bekleme süresi dolarsa meşgul iş hatası döner.
    /// </summary>
    Task<CommandResult> RunAsync(Func<CancellationToken, Task<CommandResult>> action, CancellationToken cancellationToken);
}
