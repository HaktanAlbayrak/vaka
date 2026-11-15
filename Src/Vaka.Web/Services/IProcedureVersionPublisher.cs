namespace Vaka.Web.Services;

public sealed record PublishOutcome(CommandResult Result, int CleaningTypeId, int VersionNo);

public interface IProcedureVersionPublisher
{
    /// <summary>
    /// Tipte taslak varsa reddedilir (tip başına tek taslak, PL-03). Yürürlükte versiyon varsa derin kopyası (faz, adım, medya bağlantısı,
    /// malzeme zorunluluğu, doküman no), yoksa boş taslak açılır. Versiyon no = tipteki en büyük no + 1, açılışta verilir.
    /// Başarıda <see cref="CommandResult.EntityId"/> yeni taslağın kimliğidir.
    /// </summary>
    Task<CommandResult> CreateDraftAsync(int cleaningTypeId, CancellationToken cancellationToken);

    /// <summary>
    /// Yayın engelleri geçerse ve PIN doğruysa, aynı veritabanı işleminde: eski Yürürlükte Arşiv, taslak Yürürlükte olur;
    /// yayın anı sunucu saati, yayınlayan işlemi yapan (F1-A2). Yanlış PIN denemesi işlem geri alınmadan sayılır.
    /// Engeller önce, PIN en son denetlenir. Başarıda <see cref="PublishOutcome.CleaningTypeId"/> ve <see cref="PublishOutcome.VersionNo"/> dolar.
    /// </summary>
    Task<PublishOutcome> PublishAsync(int versionId, int actorId, string? pin, CancellationToken cancellationToken);
}
