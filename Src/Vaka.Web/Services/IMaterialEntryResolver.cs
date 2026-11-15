using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed record MaterialEdits(int? RemoveIndex, int? AddLotId, string? AddExpiredReason);

public interface IMaterialEntryResolver
{
    /// <summary>
    /// Açılış formunda gönderilen lot satırlarını görünür alanlarıyla doldurur; bulunmayan lot atlanır. <paramref name="edits"/> doluysa
    /// (JS kapalı yenileme) önce <c>RemoveIndex</c> satırı çıkarılır, sonra <c>AddLotId</c> listede yoksa eklenir.
    /// </summary>
    Task<MaterialEntryViewModel[]> ResolveAsync(IReadOnlyList<MaterialEntryViewModel> posted, MaterialEdits? edits, CancellationToken cancellationToken);
}
