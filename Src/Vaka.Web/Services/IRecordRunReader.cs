using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

/// <summary>
/// Yürütme ekranının istek bağlamı: JS kapalı lot süzgeci ve reddedilen malzeme isteğinin seçili lotu,
/// JS kapalı görevli süzgeci (<c>?stepId=&amp;people=</c>) ve reddedilen görevli isteğinin seçili kişisi.
/// </summary>
public sealed record RunViewOptions(
    string? LotQuery = null,
    int? PendingLotId = null,
    int? ParticipantStepId = null,
    string? ParticipantQuery = null,
    PendingParticipantViewModel? PendingParticipant = null);

public interface IRecordRunReader
{
    /// <summary>
    /// Yürütme ekranı (E-15). Önce kaydın zaman aşımı uygulanır (BR-26); görüntüleme başka operatöre de açıktır, işlemler sahibe ve adımın görevlisine (PL-12, SK-1).
    /// Malzeme yönetilemiyorsa (sahip değil, durum Açıldı değil) lot listesi boş gelir; listede kayda eklenmiş lot yer almaz (F4-A2).
    /// Görevli adayları yalnız <c>ParticipantStepId</c> adımı görüntüleyen tarafından yönetilebiliyorsa dolar, en çok 50 kişi; reddedilen isteğin seçili kişisi listede tutulur.
    /// <c>CommandId</c> her çizimde yenidir, çağıran gerekirse geri yazar.
    /// </summary>
    Task<LookupResult<RecordRunViewModel>> GetAsync(int recordId, int viewerUserId, RunViewOptions options, CancellationToken cancellationToken);
}
