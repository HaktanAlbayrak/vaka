using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.ViewModels.Records;

public sealed class RecordCreateViewModel
{
    public Guid CommandId { get; set; }

    public int? FacilityId { get; set; }

    public int? LineId { get; set; }

    public int? MachineId { get; set; }

    public int? CleaningTypeId { get; set; }

    public string WorkOrderNo { get; set; } = string.Empty;

    public string Note { get; set; } = string.Empty;

    public int LockMinutes { get; set; }

    public int? DefaultLockMinutes { get; set; }

    public int MaxLockMinutes { get; set; }

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

    public CleaningTypeOptionViewModel[] CleaningTypes { get; set; } = [];

    public string? ErrorMessage { get; set; }

    public string? ErrorUrl { get; set; }
}
