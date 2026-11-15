using Vaka.Web.Services;

namespace Vaka.Web.ViewModels.Records;

public sealed class RecordRunViewModel
{
    public int Id { get; set; }

    public string RecordNo { get; set; } = string.Empty;

    public CleaningTypeKind Kind { get; set; }

    public string KindLabel { get; set; } = string.Empty;

    public RecordStatus Status { get; set; }

    public string StatusLabel { get; set; } = string.Empty;

    public string? FieldBookRef { get; set; }

    public string FacilityLabel { get; set; } = string.Empty;

    public string LineLabel { get; set; } = string.Empty;

    public string MachineLabel { get; set; } = string.Empty;

    public string CleaningTypeName { get; set; } = string.Empty;

    public int VersionNo { get; set; }

    public string OwnerName { get; set; } = string.Empty;

    public DateTime OpenedAtLocal { get; set; }

    public DateTime? LockExpiresAtLocal { get; set; }

    public string WorkOrderNo { get; set; } = string.Empty;

    public string Note { get; set; } = string.Empty;

    public bool MaterialRequired { get; set; }

    public bool IsOwner { get; set; }

    public bool CanManageMaterials { get; set; }

    public Guid CommandId { get; set; }

    public RecordMaterialViewModel[] Materials { get; set; } = [];

    public RecordPhaseViewModel[] Phases { get; set; } = [];

    public string? LotQuery { get; set; }

    public LotOptionViewModel[] AvailableLots { get; set; } = [];

    public bool AvailableLotsTruncated { get; set; }

    public PendingMaterialViewModel? PendingMaterial { get; set; }

    public string? ErrorMessage { get; set; }
}
