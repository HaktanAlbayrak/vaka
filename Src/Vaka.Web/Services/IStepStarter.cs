namespace Vaka.Web.Services;

public sealed record StartStepRequest(Guid CommandId, string? Pin);

/// <summary>
/// Adım başlatma (SK-1, BR-11, BR-14, BR-15, BR-24, BR-39, BR-46). Başarıda <c>EntityId</c> kayıt kimliğidir; aynı <c>CommandId</c> tek etki (BR-33).
/// Reddedilen istek olay ve komut satırı bırakmaz; PIN reddi yalnız hatalı deneme sayacını yazar.
/// </summary>
public interface IStepStarter
{
    /// <summary>
    /// Sıra: komut tekrarı, başlatanın molası, kayıt ve adım bulunur, başlatan sahip ya da adımın görevlisi (değilse <see cref="CommandStatus.Forbidden"/>),
    /// kayıt Açıldı ya da Devam ediyor (kilit bitişi geçmişse Zaman aşımı, BR-46), kayıtta açık mola yok (BR-39), adım Bekliyor,
    /// kayıtta devam eden adım yok (BR-15), önceki adımların hepsi tamamlanmış (BR-14), ilk adımda malzeme zorunluysa en az bir malzeme (BR-11),
    /// görevli var ve hiçbiri pasif, molada ya da başka kayıtta devam eden adımda değil (BR-24), PIN en son.
    /// Başarıda adım Devam ediyor, görevliler çalışıyor, faz Devam ediyor olur; ilk adımda kayıt Devam ediyor olur, başlangıç anı yazılır ve planlıda
    /// saha referansı üretilir (BR-09). Eşzamanlı başlatmada kısmi benzersiz index ihlali nedenli ret olur.
    /// </summary>
    Task<CommandResult> StartAsync(int stepId, int actorUserId, StartStepRequest request, CancellationToken cancellationToken);
}
