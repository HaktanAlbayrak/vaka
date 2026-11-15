using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

/// <summary>Adım başlatmanın durum değişikliği; normal başlatma (<see cref="IStepStarter"/>) ve acil açılış aynı kodu kullanır.</summary>
public interface IStepStartApplier
{
    /// <summary>
    /// Adımın görevlileri (BR-24): en az bir görevli var, hiçbiri pasif, molada ya da başka kayıtta devam eden adımda değil.
    /// Sorun yoksa boş, varsa nedenli ret döner.
    /// </summary>
    Task<CommandResult?> CheckParticipantsAsync(RecordStep step, CancellationToken cancellationToken);

    /// <summary>
    /// Adımı <paramref name="now"/> anında başlatır: adım ve faz Devam ediyor, görevliler çalışıyor; ilk adımda kayıt Devam ediyor olur,
    /// başlangıç anı yazılır ve planlı kayıtta saha referansı üretilir (BR-09), acilde üretilmez (BR-50). Değişiklik ve <c>StepStarted</c> olayı
    /// kaydedilir; komut satırını çağıran ekler. Eşzamanlı başlatmada görevli kısmi benzersiz index ihlali nedenli ret olur (boş dönerse başarılı).
    /// </summary>
    Task<CommandResult?> ApplyAsync(CleaningRecord record, RecordStep step, bool isFirst, int actorUserId, Guid commandId, DateTime now, CancellationToken cancellationToken);
}
