namespace Vaka.Web.Services;

public sealed record MediaUploadRequest(string? FileName, long Length, Stream Content);

public sealed record MediaFile(string Sha256, string ContentType, long SizeBytes, string OriginalFileName);

public interface IStepMediaService
{
    /// <summary>
    /// Biçim uzantıyla ve baştaki imza baytlarıyla birlikte doğrulanır (JPEG, PNG, WebP fotoğraf; MP4 video, K-26).
    /// Sınır: Boyut &lt;= AzamiMB x 1 048 576 bayt, türe göre fotoğraf ya da video ayarı, her istekte okunur (PL-47, PL-51).
    /// Hatalar <c>File</c> alanıyla döner; <see cref="CommandResult.EntityId"/> versiyon kimliğidir.
    /// </summary>
    Task<CommandResult> UploadAsync(int stepId, MediaUploadRequest? upload, CancellationToken cancellationToken);

    /// <summary>Yalnız bağlantı satırı çıkar; dosya silinmez (başka versiyon paylaşabilir).</summary>
    Task<CommandResult> RemoveAsync(int mediaId, CancellationToken cancellationToken);

    /// <summary>Medyanın bağlı olduğu adım; hata hedefi <c>media:&lt;stepId&gt;</c> için. Medya yoksa null.</summary>
    Task<int?> FindStepIdAsync(int mediaId, CancellationToken cancellationToken);

    /// <summary>Taslak versiyonun medyası <paramref name="includeDraft"/> false ise bulunamamış sayılır (Denetçi taslağı göremez, PL-51).</summary>
    Task<LookupResult<MediaFile>> FindAsync(int mediaId, bool includeDraft, CancellationToken cancellationToken);
}
