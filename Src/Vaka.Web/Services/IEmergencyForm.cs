using Vaka.Web.ViewModels.Emergencies;

namespace Vaka.Web.Services;

public interface IEmergencyForm
{
    /// <summary>
    /// Acil formunu listeleriyle kurar. <paramref name="posted"/> boşsa yeni <c>CommandId</c> üretilir; doluysa gönderilen alanlar ve
    /// <c>CommandId</c> korunur, tutarsız seçim (başka tesisin hattı gibi) boşaltılır; acil tipi geçerli değilse boşaltılır. PIN korunmaz.
    /// <paramref name="applyMaterialEdits"/> true iken JS kapalı lot ekle ve çıkar uygulanır.
    /// </summary>
    Task<EmergencyCreateViewModel> BuildAsync(EmergencyCreateViewModel? posted, bool applyMaterialEdits, CancellationToken cancellationToken);
}
