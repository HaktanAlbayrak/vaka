namespace Vaka.Web.Services;

public sealed record AddParticipantRequest(Guid CommandId, int ParticipantUserId, string? Pin);

public sealed record RemoveParticipantRequest(Guid CommandId, int ParticipantUserId, string? Pin);

public sealed record JoinStepRequest(Guid CommandId, int ParticipantUserId, string? Pin);

public sealed record LeaveStepRequest(Guid CommandId, int ParticipantUserId, string? Pin);

/// <summary>
/// Adım görevlileri (SK-2, BR-53). Yalnız kayıt sahibi, adım Bekliyor ya da Devam ediyorken ve kendi PIN'iyle ekler ya da çıkarır; çıkarılan kişi adıma hiç katılmamış olmalıdır; sahip değilse
/// <see cref="CommandStatus.Forbidden"/>, adım yoksa <see cref="CommandStatus.NotFound"/>. Başarıda <c>EntityId</c> kayıt kimliğidir.
/// Eklenen kişinin girişi ayrı işlemdir (<see cref="JoinAsync"/>, O-10). Kayıtta ya da sahipte açık mola varken ve adım bittikten sonra istek reddedilir (BR-39). Sahip kendini adım sürerken eklerse aynı istekte katılmış olur.
/// Reddedilen istek olay ve komut satırı bırakmaz; aynı <c>CommandId</c> tek etki (BR-33).
/// </summary>
public interface IParticipantService
{
    /// <summary>Pasif, Operatör olmayan ya da açık molası olan kişi eklenemez; kişi adımda zaten görevliyse ret. Kendi kaydı Devam eden kişi eklenebilir (K-41 yalnız uyarıdır).</summary>
    Task<CommandResult> AddAsync(int stepId, int actorUserId, AddParticipantRequest request, CancellationToken cancellationToken);

    /// <summary>Adımda en az bir görevli kalmalıdır; sahip kendini de çıkarabilir (K5). Adıma katılmış (aralığı olan ya da çalışan) kişi çıkarılamaz, ayrılır.</summary>
    Task<CommandResult> RemoveAsync(int stepId, int actorUserId, RemoveParticipantRequest request, CancellationToken cancellationToken);

    /// <summary>Oturumdaki kullanıcı sahip ya da kişinin kendisi olmalıdır (değilse Forbidden); PIN kişinin PIN'idir (Faz 6E).</summary>
    Task<CommandResult> JoinAsync(int stepId, int actorUserId, JoinStepRequest request, CancellationToken cancellationToken);

    /// <summary>PIN ayrılan kişinin PIN'idir; adımda çalışan son kişi ayrılamaz (Faz 6E).</summary>
    Task<CommandResult> LeaveAsync(int stepId, int actorUserId, LeaveStepRequest request, CancellationToken cancellationToken);
}
