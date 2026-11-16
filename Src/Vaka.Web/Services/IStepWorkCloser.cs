using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

/// <summary>Adım kişi başına çalışma süreleri: bitişte ya da müdahalede çalışan kişilerin kullanıcı kimlikleri ve SH-02a saniyeleri.</summary>
public sealed record StepWorkResult(int[] UserIds, int[] WorkSeconds);

/// <summary>Adım biterken ya da kesilirken kişi aralıklarını kapatır ve kişi çalışma sürelerini yazar (SH-02a, O-11).</summary>
public interface IStepWorkCloser
{
    /// <summary>
    /// Adımın görevlilerinin aralıklarını <paramref name="now"/> anında <paramref name="kind"/> ile kapatır. Açık aralık kapanır; Faz 6E öncesinden
    /// kalan, aralık satırı olmayan ama çalışan görevli için adım başlangıcından başlayan kapalı aralık eklenir (adımın tamamında çalışmış sayılır);
    /// önceden ayrılmış kişinin aralığı olduğu gibi katılır; hiç katılmamış kişi sayılmaz. Çalışan görevlilerin <c>IsWorking</c> değeri kapanır.
    /// Her kişinin süresi SH-02a ile hesaplanır. Katılmış görevliler yüklü olmalıdır (<c>WorkInterval</c>); değişiklikler kaydedilmez.
    /// </summary>
    StepWorkResult Close(RecordStep step, StepIntervalEndKind kind, int actorUserId, DateTime now, IReadOnlyList<BreakSpan> breaks);
}
