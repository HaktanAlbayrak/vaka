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

    public RecordBreakViewModel[] Breaks { get; set; } = [];

    public OpenBreakViewModel? OpenBreak { get; set; }

    /// <summary>Kayıtta açık mola yokken true; false iken ekran sayaçları ilerletmez ve "Sayaçlar durdu" yazar.</summary>
    public bool TimersRunning { get; set; }

    public bool CanStartBreak { get; set; }

    public string? CannotBreakReason { get; set; }

    public RecordSummaryViewModel Summary { get; set; } = new();

    public string? FirstStepStartedByName { get; set; }

    /// <summary>JS kapalı görevli ekleme: <c>?stepId=&amp;people=</c> süzgeci.</summary>
    public int? ParticipantStepId { get; set; }

    public string? ParticipantQuery { get; set; }

    /// <summary>En çok 50 kişi; yalnız <c>ParticipantStepId</c> doluysa ve adımın görevlisi yönetilebiliyorsa dolar.</summary>
    public ParticipantOptionViewModel[] ParticipantOptions { get; set; } = [];

    public bool ParticipantOptionsTruncated { get; set; }

    public PendingParticipantViewModel? PendingParticipant { get; set; }
}
