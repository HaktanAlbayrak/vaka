using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Services;

/// <summary>Rapor, liste ve izlenebilirlik süzgeçlerinin seçenekleri; pasif ve kullanımdan kalkmış tanımlar da geçmiş kayıtlar için listelenir.</summary>
public interface IReportOptions
{
    Task<SelectOptionViewModel[]> ListFacilitiesAsync(CancellationToken cancellationToken);

    /// <summary>Tesis seçilmemişse boş.</summary>
    Task<SelectOptionViewModel[]> ListLinesAsync(int? facilityId, CancellationToken cancellationToken);

    /// <summary>Tesis ya da hat seçilmemişse boş; hat seçiliyse yalnız o hattın, değilse tesisin makineleri.</summary>
    Task<SelectOptionViewModel[]> ListMachinesAsync(int? facilityId, int? lineId, CancellationToken cancellationToken);

    /// <summary>Tüm makineler, etiket <c>Tesis-Hat-Makine - Ad</c> biçiminde (kayıt no öneki ile aynı).</summary>
    Task<SelectOptionViewModel[]> ListAllMachinesAsync(CancellationToken cancellationToken);

    Task<SelectOptionViewModel[]> ListCleaningTypesAsync(CancellationToken cancellationToken);

    /// <summary>Seçilen tesis, hat, makine ve temizlik tipi kimliklerinin varlığını ve birbirine uygunluğunu denetler; sorun varsa iletiyi, yoksa boş döner.</summary>
    Task<string?> FindInvalidSelectionAsync(int? facilityId, int? lineId, int? machineId, int? cleaningTypeId, CancellationToken cancellationToken);
}
