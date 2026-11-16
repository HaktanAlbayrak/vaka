using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed record ParticipantSearchResult(ParticipantOptionViewModel[] Options, bool Truncated);

public interface IParticipantDirectory
{
    /// <summary>
    /// Görevli adayları (PL-88): Operatör rolündeki aktif kullanıcılar; ad ya da sicil no içinde <paramref name="query"/> aranır,
    /// Türkçe kurala göre büyük-küçük harf duyarsız ("İ" ile "i", "I" ile "ı" eşleşir). Ada göre sıralı, en çok 50 satır.
    /// Adımda zaten görevli olanlar listelenmez. Açık molası olan kişi listelenir, <c>IsAvailable</c> false ve neden "Molada" yazılıdır (BR-53).
    /// Kendi kaydı Devam eden kişi uygundur, <c>HasOwnActiveRecord</c> ile K-41 uyarısı için işaretlenir.
    /// <paramref name="includeUserId"/> doluysa, süzgeçlere uyuyor ve listede yoksa sona eklenir (reddedilen isteğin seçili kişisi).
    /// </summary>
    Task<ParticipantSearchResult> SearchAsync(string? query, int? stepId, int? includeUserId, CancellationToken cancellationToken);

    /// <summary>
    /// <see cref="SearchAsync"/> çağrısını yalnız kaydın sahibine açar: adım yoksa NotFound, istek sahibi kaydın sahibi değilse Forbidden (K-30).
    /// </summary>
    Task<LookupResult<ParticipantSearchResult>> SearchForOwnerAsync(int stepId, int actorUserId, string? query, CancellationToken cancellationToken);
}
