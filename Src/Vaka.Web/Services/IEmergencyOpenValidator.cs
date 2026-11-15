namespace Vaka.Web.Services;

public interface IEmergencyOpenValidator
{
    /// <summary>
    /// Sunucu doğrulaması (BR-01, BR-11, BR-12, BR-50): makine ve hiyerarşi tutarlı ve aktif, tip Acil türünde ve yürürlükte versiyonlu
    /// (makine eşlemesi aranmaz), açıklama 1-1000 karakter, tip malzeme istiyorsa en az bir lot, SKT geçmiş lota gerekçe.
    /// </summary>
    Task<EmergencyValidation> ValidateAsync(EmergencyOpenRequest request, CancellationToken cancellationToken);
}
