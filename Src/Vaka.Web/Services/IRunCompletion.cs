using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

public sealed record PhaseTotals(int GrossSeconds, int BreakSeconds, int NetSeconds, bool BelowMinimum);

/// <summary>Faz ve kaydın adım bitişinde otomatik tamamlanması (BR-21, BR-22).</summary>
public interface IRunCompletion
{
    /// <summary>
    /// Fazın bitiş anındaki toplamları: brüt (SH-03), mola (SH-04), net (SH-06) ve minimum kontrolü (SH-07).
    /// Faz başlangıcı fazın adımlarının en erken başlangıcıdır; <paramref name="phaseEnd"/> bitirilen son adımın bitiş anıdır.
    /// </summary>
    PhaseTotals MeasurePhase(RecordPhase phase, DateTime phaseEnd, IReadOnlyList<BreakSpan> breaks);

    /// <summary>
    /// Fazı tamamlar: durum Tamamlandı, süre altındaysa Tamamlandı - süre altında ve gerekçe; olay <c>PhaseCompleted</c>.
    /// Fazın hepsi tamamlanmışsa kaydı da tamamlar: tamamlanma anı, brüt (SH-08), mola (SH-09), net (SH-11), efor (SH-12), Version artar,
    /// olay <c>RecordCompleted</c>; makine kilidi Durum Tamamlandı olunca kısmi benzersiz index'ten düşer (BR-22).
    /// Çağıran adımın bitiş alanlarını kaydetmiş olmalıdır (efor toplamı adımlardan okunur).
    /// </summary>
    Task CompleteAsync(CleaningRecord record, RecordPhase phase, PhaseTotals totals, string? reason, IReadOnlyList<BreakSpan> breaks, int actorUserId, Guid commandId, DateTime now, CancellationToken cancellationToken);
}
