namespace Vaka.Web.Services;

public sealed record FinishStepRequest(Guid CommandId, string? Pin, string? BelowMinimumReason);

/// <summary>Fazın son adımı süre altında bitirilirken gerekçe formunu çizmek için gereken değerler (BR-21).</summary>
public sealed record BelowMinimumPrompt(int RecordId, string PhaseTitle, int PhaseNetSeconds, int MinDurationSeconds);

/// <summary><paramref name="Prompt"/> doluysa istek faz süre altı kuralına takıldı ya da gerekçe formu üzerinden reddedildi; controller 200 gerekçe formunu çizer.</summary>
public sealed record FinishOutcome(CommandResult Result, BelowMinimumPrompt? Prompt);

public interface IStepFinisher
{
    /// <summary>
    /// Adım bitirme (SK-1, BR-21, BR-22, BR-39). Başarıda <c>EntityId</c> kayıt kimliğidir; aynı <c>CommandId</c> tek etki (BR-33).
    /// Sıra: komut tekrarı, bitirenin molası, başlatan sahip ya da adımın görevlisi (<see cref="CommandStatus.Forbidden"/>), kayıt Devam ediyor,
    /// kayıtta açık mola yok, adım Devam ediyor, fazın son adımıysa faz net süresi (SH-06) minimumun altındayken gerekçe (en çok 500 karakter), PIN en son.
    /// Gerekçesiz süre altı istek olay ve komut bırakmaz, PIN denenmez; gerekçeli gönderimde bitiş zamanı o isteğin sunucu saatidir.
    /// Bitişte adım süresi (SH-01), eforu (SH-02) yazılır, görevliler boşa çıkar; faz ve kayıt otomatik tamamlanır, makine kilidi kalkar (BR-22).
    /// </summary>
    Task<FinishOutcome> FinishAsync(int stepId, int actorUserId, FinishStepRequest request, CancellationToken cancellationToken);
}
