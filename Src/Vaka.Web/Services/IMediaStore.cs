namespace Vaka.Web.Services;

public sealed record StoredMedia(string Sha256, long SizeBytes);

public interface IMediaStore
{
    /// <summary>
    /// İçeriği SHA-256 özetiyle adlandırılmış dosyaya yazar; aynı içerik tek dosyadır. <paramref name="maxBytes"/> aşılırsa
    /// yazma bırakılır ve null döner. Gerçek bayt sayısı beyan edilen uzunluğa güvenmeden sayılır.
    /// </summary>
    Task<StoredMedia?> SaveAsync(Stream content, long maxBytes, CancellationToken cancellationToken);

    /// <summary>Dosya yoksa null.</summary>
    Stream? Open(string sha256);
}
