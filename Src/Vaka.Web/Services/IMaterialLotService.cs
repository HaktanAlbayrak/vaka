using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Services;

public sealed record MaterialLotRequest(string? LotNo, DateOnly? ExpiryDate);

public interface IMaterialLotService
{
    /// <summary>Kayıtlı lot için <paramref name="id"/>, yeni lot için <paramref name="materialId"/> verilir; kayıt bulunmazsa NotFound.</summary>
    Task<LookupResult<MaterialLotFormViewModel>> GetFormAsync(int? id, int? materialId, CancellationToken cancellationToken);

    /// <summary>Lot no malzeme içinde benzersizdir. Başarıda <c>EntityId</c> malzemenin kimliğidir.</summary>
    Task<CommandResult> CreateAsync(int materialId, MaterialLotRequest request, CancellationToken cancellationToken);

    /// <summary>Başarıda <c>EntityId</c> malzemenin kimliğidir.</summary>
    Task<CommandResult> UpdateAsync(int id, MaterialLotRequest request, CancellationToken cancellationToken);
}
