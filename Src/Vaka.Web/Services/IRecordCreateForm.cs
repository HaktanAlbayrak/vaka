using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public interface IRecordCreateForm
{
    /// <summary>
    /// Kayıt açma formunu listeleriyle kurar. <paramref name="posted"/> boşsa yeni <c>CommandId</c> üretilir; doluysa gönderilen alanlar ve
    /// <c>CommandId</c> korunur, tutarsız seçim (başka tesisin hattı gibi) boşaltılır. <paramref name="applyMaterialEdits"/> true iken
    /// JS kapalı lot ekle (<c>AddMaterialLotId</c>) ve çıkar (<c>RemoveMaterialIndex</c>) uygulanır. PIN korunmaz.
    /// </summary>
    Task<RecordCreateViewModel> BuildAsync(RecordCreateViewModel? posted, bool applyMaterialEdits, CancellationToken cancellationToken);
}
