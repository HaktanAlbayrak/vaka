using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

/// <summary>Adım başlatmanın durum değişikliği; normal başlatma (<see cref="IStepStarter"/>) ve acil açılış aynı kodu kullanır.</summary>
public interface IStepStartApplier
{
    /// <summary>
    /// Adımın görevlileri (BR-24, Faz 6E): en az bir görevli var; başlatan ve listedeki sahip pasif, molada ya da başka kayıtta devam eden adımda değil;
    /// çalışmaya başlayacak en az bir kişi var. Girişini yapmış başka bir kişinin uygunsuzluğu başlatmayı engellemez. Sorun yoksa boş, varsa nedenli ret döner.
    /// </summary>
    Task<CommandResult?> CheckParticipantsAsync(RecordStep step, int starterUserId, int ownerUserId, CancellationToken cancellationToken);

    /// <summary>
    /// Adımı <paramref name="now"/> anında başlatır: adım ve faz Devam ediyor, çalışmaya başlayan görevlilerin aralığı açılır ve çalışıyor olurlar (başlatan, sahip, girişini yapmış uygun kişiler); ilk adımda kayıt Devam ediyor olur,
    /// başlangıç anı yazılır ve planlı kayıtta saha referansı üretilir (BR-09), acilde üretilmez (BR-50). Değişiklik ve <c>StepStarted</c> olayı
    /// kaydedilir; komut satırını çağıran ekler. Eşzamanlı başlatmada görevli kısmi benzersiz index ihlali nedenli ret olur (boş dönerse başarılı).
    /// </summary>
    Task<CommandResult?> ApplyAsync(CleaningRecord record, RecordStep step, bool isFirst, int actorUserId, Guid commandId, DateTime now, CancellationToken cancellationToken);
}
