using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed record RecordListQuery(
    string? Query,
    int? FacilityId,
    int? LineId,
    int? MachineId,
    CleaningTypeKind? Kind,
    RecordStatus? Status,
    DateOnly? From,
    DateOnly? To,
    bool MineOnly,
    int Page,
    int PageSize);

public interface IRecordListReader
{
    /// <summary>
    /// Kayıt listesi (E-19). Önce kilit bitişi geçmiş Açıldı kayıtlar Zaman aşımına alınır (BR-26). Süzgeçler birlikte çalışır (VE).
    /// <c>Query</c> kayıt no, saha referansı, iş emri ve kayda eklenmiş (çıkarılmamış) malzemenin lot numarasında büyük küçük harf gözetmeden içerir araması yapar (Ö-06).
    /// <c>MineOnly</c> kayıt sahibi ya da herhangi bir adımın görevlisi olunan kayıtları getirir (K-17, PL-08).
    /// Tarih aralığı açılış anının Türkiye takvim gününe göre, iki uç dahildir. Sıra: en yeni açılış önce. Hat seçenekleri yalnız seçili tesisin, makine seçenekleri yalnız seçili hat ya da tesisin makineleridir (seçim yoksa boş). Sayfalama sunucuda; sayfa ve boyut sınırlanır (1-100).
    /// </summary>
    Task<RecordListViewModel> ListAsync(RecordListQuery query, int viewerUserId, CancellationToken cancellationToken);
}
