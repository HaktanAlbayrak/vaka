namespace Vaka.Web.Services;

public sealed record StartBreakRequest(Guid CommandId, string? Pin);

public sealed record EndBreakRequest(Guid CommandId, string? Pin);

/// <summary>
/// Adım arası mola (BR-38, BR-39, BR-40). Başarıda <c>EntityId</c> kayıt kimliğidir; aynı <c>CommandId</c> ile tekrar istek ilk sonucu döner (BR-33).
/// Reddedilen istek olay ve komut satırı bırakmaz. Molada olan kişi başlatamaz: <see cref="CommandStatus.OnBreak"/>.
/// </summary>
public interface IBreakService
{
    /// <summary>
    /// Koşullar: kayıt Devam ediyor, devam eden adım yok (adım sürerken mola Faz 7, PL-57), kayıtta açık mola yok;
    /// başlatan sahip ya da sıradaki (ilk tamamlanmamış) adımın görevlisi, değilse <see cref="CommandStatus.Forbidden"/> (SK-6);
    /// başlatan başka kayıtta devam eden adımda değil (K14a); PIN en son. Eşzamanlı istekte kısmi benzersiz index ihlali nedenli ret olur.
    /// </summary>
    Task<CommandResult> StartAsync(int recordId, int actorUserId, StartBreakRequest request, CancellationToken cancellationToken);

    /// <summary>Yalnız başlatan bitirir, değilse <see cref="CommandStatus.Forbidden"/> (BR-40); kapanış türü <see cref="BreakEndKind.Person"/>. Molada olan kişi kendi molasını bitirebilir.</summary>
    Task<CommandResult> EndAsync(int breakId, int actorUserId, EndBreakRequest request, CancellationToken cancellationToken);
}
