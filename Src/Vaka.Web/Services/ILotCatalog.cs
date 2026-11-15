using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed record LotSearchResult(LotOptionViewModel[] Lots, bool Truncated);

public interface ILotCatalog
{
    /// <summary>
    /// Aktif malzemelerin lotları, makine ve tesisten bağımsız; en çok 50 satır (PL-64), malzeme kodu ve lot no sırasıyla.
    /// <paramref name="query"/> malzeme kodu, adı ya da lot no içinde aranır; boşsa ilk 50 döner. Süre geçmiş lot <c>IsExpired</c> ile işaretlenir (BR-12).
    /// <paramref name="includeLotId"/> doluysa ve aktif malzemeye aitse listede yoksa sona eklenir (reddedilen isteğin seçili lotu).
    /// <paramref name="excludeRecordId"/> doluysa o kayda eklenmiş (çıkarılmamış) lotlar listelenmez, seçili lot olarak da eklenmez (F4-A2).
    /// </summary>
    Task<LotSearchResult> FindAsync(string? query, int? includeLotId, int? excludeRecordId, CancellationToken cancellationToken);

    /// <summary>Verilen kimliklerin lotları (malzeme aktifliğine bakmaz); bulunmayan kimlik atlanır.</summary>
    Task<LotOptionViewModel[]> GetAsync(IReadOnlyCollection<int> lotIds, CancellationToken cancellationToken);
}
