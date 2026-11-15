namespace Vaka.Web.Services;

public sealed record AddParticipantRequest(Guid CommandId, int ParticipantUserId, string? Pin);

public sealed record RemoveParticipantRequest(Guid CommandId, int ParticipantUserId, string? Pin);

/// <summary>
/// Adım görevlileri (SK-2, BR-53). Yalnız kayıt sahibi, adım başlamadan ve kendi PIN'iyle ekler ya da çıkarır; sahip değilse
/// <see cref="CommandStatus.Forbidden"/>, adım yoksa <see cref="CommandStatus.NotFound"/>. Başarıda <c>EntityId</c> kayıt kimliğidir.
/// Eklenen kişinin PIN'i bu fazda istenmez, katılım onayı Faz 7'dedir (K-10, PL-57). Kayıtta ya da sahipte açık mola varken ve adım başladıktan sonra istek reddedilir (BR-39).
/// Reddedilen istek olay ve komut satırı bırakmaz; aynı <c>CommandId</c> tek etki (BR-33).
/// </summary>
public interface IParticipantService
{
    /// <summary>Pasif, Operatör olmayan ya da açık molası olan kişi eklenemez; kişi adımda zaten görevliyse ret. Kendi kaydı Devam eden kişi eklenebilir (K-41 yalnız uyarıdır).</summary>
    Task<CommandResult> AddAsync(int stepId, int actorUserId, AddParticipantRequest request, CancellationToken cancellationToken);

    /// <summary>Adımda en az bir görevli kalmalıdır; sahip kendini de çıkarabilir (K5).</summary>
    Task<CommandResult> RemoveAsync(int stepId, int actorUserId, RemoveParticipantRequest request, CancellationToken cancellationToken);
}
