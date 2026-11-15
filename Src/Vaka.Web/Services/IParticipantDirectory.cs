using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed record ParticipantSearchResult(ParticipantOptionViewModel[] Options, bool Truncated);

public interface IParticipantDirectory
{
    /// <summary>
    /// Görevli adayları: Operatör rolündeki kullanıcılar, ad ya da sicil no içinde <paramref name="query"/> aranır; ada göre sıralı, en çok 50 satır (PL-64).
    /// Aday uygun değilse <c>IsAvailable</c> false ve neden yazılıdır: pasif, adımda zaten görevli ya da açık molası olan kişi (BR-53).
    /// Kendi kaydı Devam eden kişi uygundur, <c>HasOwnActiveRecord</c> ile K-41 uyarısı için işaretlenir.
    /// <paramref name="includeUserId"/> doluysa ve listede yoksa sona eklenir (reddedilen isteğin seçili kişisi).
    /// </summary>
    Task<ParticipantSearchResult> SearchAsync(string? query, int? stepId, int? includeUserId, CancellationToken cancellationToken);
}
