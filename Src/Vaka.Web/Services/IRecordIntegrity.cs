using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed record EventCheck(int Sequence, string EventType, DateTime OccurredAtUtc, string Hash, bool IsValid, string? Problem);

public sealed record IntegrityResult(
    bool IsValid,
    int? FirstBrokenSequence,
    IReadOnlyList<EventCheck> Events,
    IReadOnlyList<IntegrityMismatchViewModel> Mismatches);

public interface IRecordIntegrity
{
    /// <summary>
    /// Kaydın bütünlük doğrulaması (BR-32, PL-11). (1) Özet zinciri: her olay için sıra bir öncekinin bir fazlası olmalı,
    /// saklanan önceki özet bir önceki olayın özetine eşit olmalı ve HMAC-SHA256(önceki özet + "\n" + kanonik olay) saklanan özete eşit olmalı.
    /// (2) Durum tablolarındaki zaman, kişi ve malzeme alanları olay değerleriyle aynı olmalı.
    /// Geçerli = zincir bozuk olay yok ve alan farkı yok. İlk tutarsız sıra = bozuk olayların ve olaya bağlı alan farklarının en küçük sırası.
    /// Kayıt yoksa boş döner. Salt okur; sonuç saklanmaz, her çağrıda yeniden hesaplanır.
    /// </summary>
    Task<IntegrityResult?> CheckAsync(int recordId, CancellationToken cancellationToken);
}
