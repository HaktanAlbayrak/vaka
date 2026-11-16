using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

/// <param name="Users">Kullanıcı kimliğine göre ad.</param>
/// <param name="StepLabels">Kayıt adımı kimliğine göre "Faz n / Adım m - başlık" etiketi.</param>
/// <param name="PhaseOrders">Kayıt fazı kimliğine göre prosedürdeki faz sırası.</param>
public sealed record TimelineNames(
    IReadOnlyDictionary<int, string> Users,
    IReadOnlyDictionary<int, string> StepLabels,
    IReadOnlyDictionary<int, int> PhaseOrders);

public interface IRecordTimelineBuilder
{
    /// <summary>
    /// Olay başına bir satır, <c>Sequence</c> sırasıyla, Türkiye gününe göre gruplu çizelge.
    /// Süre ve efor yalnız olay yükünde saklı saniyelerdir, burada süre hesaplanmaz (G8).
    /// Satırın günü = yerel(OccurredAtUtc).Tarih, yerel = UTC + Türkiye saat farkı (K-32).
    /// </summary>
    RecordTimelineDayViewModel[] Build(IReadOnlyList<ParsedEvent> events, TimelineNames names);
}
