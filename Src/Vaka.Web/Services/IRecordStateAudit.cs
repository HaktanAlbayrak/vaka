using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public interface IRecordStateAudit
{
    /// <summary>
    /// Kaydın durum tablolarındaki zaman, kişi ve malzeme alanlarını olay günlüğündeki değerlerle karşılaştırır (PL-11, SC-21).
    /// Fark yoksa boş liste döner. Olayın bulunamadığı alan sıra 0 ile bildirilir.
    /// </summary>
    Task<IReadOnlyList<IntegrityMismatchViewModel>> FindMismatchesAsync(int recordId, IReadOnlyList<ParsedEvent> events, CancellationToken cancellationToken);
}
