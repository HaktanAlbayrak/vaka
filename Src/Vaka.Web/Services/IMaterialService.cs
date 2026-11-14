using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Services;

public sealed record MaterialRequest(string? Code, string? Name, bool IsActive);

public interface IMaterialService
{
    Task<MaterialListViewModel> ListAsync(string? query, bool showInactive, int page, CancellationToken cancellationToken);

    Task<LookupResult<MaterialFormViewModel>> GetFormAsync(int? id, CancellationToken cancellationToken);

    /// <summary>Lotlar SKT sırasındadır; <c>IsExpired</c> <see cref="IDurationCalculator.IsLotExpired"/> ile hesaplanır (BR-12).</summary>
    Task<LookupResult<MaterialDetailViewModel>> GetDetailAsync(int id, CancellationToken cancellationToken);

    Task<CommandResult> CreateAsync(MaterialRequest request, CancellationToken cancellationToken);

    Task<CommandResult> UpdateAsync(int id, MaterialRequest request, CancellationToken cancellationToken);
}
