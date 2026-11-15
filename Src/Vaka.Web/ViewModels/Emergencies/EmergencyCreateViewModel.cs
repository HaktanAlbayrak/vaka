using Vaka.Web.ViewModels.Definitions;
using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.ViewModels.Emergencies;

/// <summary>
/// Formdan okunan alanlar: <c>CommandId</c>, <c>FacilityId</c>, <c>LineId</c>, <c>MachineId</c>, <c>CleaningTypeId</c>, <c>Description</c>,
/// <c>Materials</c>, <c>Pin</c> ve JS kapalı yenileme alanları (<c>LotQuery</c>, <c>AddMaterialLotId</c>, <c>AddExpiredReason</c>, <c>RemoveMaterialIndex</c>).
/// Seçenek listeleri sunucu tarafından doldurulur.
/// </summary>
public sealed class EmergencyCreateViewModel
{
    public Guid CommandId { get; set; }

    public int? FacilityId { get; set; }

    public int? LineId { get; set; }

    public int? MachineId { get; set; }

    public int? CleaningTypeId { get; set; }

    public string Description { get; set; } = string.Empty;

    public MaterialEntryViewModel[] Materials { get; set; } = [];

    public string Pin { get; set; } = string.Empty;

    public string? LotQuery { get; set; }

    public LotOptionViewModel[] AvailableLots { get; set; } = [];

    public bool AvailableLotsTruncated { get; set; }

    public int? AddMaterialLotId { get; set; }

    public string? AddExpiredReason { get; set; }

    public int? RemoveMaterialIndex { get; set; }

    public SelectOptionViewModel[] Facilities { get; set; } = [];

    public SelectOptionViewModel[] Lines { get; set; } = [];

    public MachineOptionViewModel[] Machines { get; set; } = [];

    public EmergencyTypeOptionViewModel[] EmergencyTypes { get; set; } = [];

    public string? ErrorMessage { get; set; }

    public string? ErrorUrl { get; set; }
}
