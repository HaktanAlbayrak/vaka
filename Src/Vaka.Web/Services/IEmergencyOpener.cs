namespace Vaka.Web.Services;

public interface IEmergencyOpener
{
    /// <summary>
    /// Acil kaydı açar ve ilk adımını başlatır (FR-51, BR-50, K-14). Sıra: komut tekrarı, açılan kişinin molası, aktif kullanıcı, alan doğrulaması,
    /// makine ve sahip kilidi (BR-23, BR-37), PIN en son; hepsi yazma kilitli tek işlemde. Kayıt açılışı ve ilk adım başlangıcı aynı sunucu anıdır
    /// (BR-13), kayıt no <c>A</c> türündedir, saha referansı üretilmez, kilit süresi yoktur. Olaylar sırayla <c>RecordOpened</c> ve <c>StepStarted</c>.
    /// Reddedilen istek kayıt, olay ve komut satırı bırakmaz (PIN reddi yalnız sayacı yazar). Başarıda <c>EntityId</c> kayıt kimliğidir;
    /// aynı <c>CommandId</c> ile tekrar istek ilk sonucu döner (BR-33).
    /// </summary>
    Task<OpenOutcome> OpenAsync(int ownerUserId, EmergencyOpenRequest request, CancellationToken cancellationToken);
}
