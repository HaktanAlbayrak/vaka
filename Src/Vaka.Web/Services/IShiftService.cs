using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Services;

public sealed record ShiftRequest(string? Name, TimeOnly? StartTime, TimeOnly? EndTime, bool IsActive);

public interface IShiftService
{
    /// <summary><paramref name="facilityId"/> boşsa ya da bulunmazsa koda göre ilk tesis seçilir; tesis yoksa <c>FacilityId</c> 0.</summary>
    Task<ShiftListViewModel> ListAsync(int? facilityId, CancellationToken cancellationToken);

    Task<LookupResult<ShiftFormViewModel>> GetFormAsync(int? id, int? facilityId, CancellationToken cancellationToken);

    /// <summary>K-38: aktif vardiya aynı tesisteki aktif vardiyayla çakışamaz. Başarıda <c>EntityId</c> tesisin kimliğidir.</summary>
    Task<CommandResult> CreateAsync(int facilityId, ShiftRequest request, CancellationToken cancellationToken);

    /// <summary>Tesis değişmez. Başarıda <c>EntityId</c> tesisin kimliğidir.</summary>
    Task<CommandResult> UpdateAsync(int id, ShiftRequest request, CancellationToken cancellationToken);
}
