using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public interface IRecordRunReader
{
    /// <summary>
    /// Yürütme ekranı (E-15, Açıldı hali). Önce kaydın zaman aşımı uygulanır (BR-26); görüntüleme başka operatöre de açıktır, işlemler yalnız sahibe (PL-12).
    /// <paramref name="lotQuery"/> JS kapalı lot süzgecidir; <paramref name="pendingLotId"/> reddedilen isteğin seçili lotunu listede tutar.
    /// Malzeme yönetilemiyorsa (sahip değil, durum Açıldı değil) lot listesi boş gelir. <c>CommandId</c> her çizimde yenidir, çağıran gerekirse geri yazar.
    /// </summary>
    Task<LookupResult<RecordRunViewModel>> GetAsync(int recordId, int viewerUserId, string? lotQuery, int? pendingLotId, CancellationToken cancellationToken);
}
