using Vaka.Web.Services;

namespace Vaka.Web.ViewModels.Records;

public sealed class RecordListItemViewModel
{
    public int Id { get; set; }

    public string RecordNo { get; set; } = string.Empty;

    public CleaningTypeKind Kind { get; set; }

    public string KindLabel { get; set; } = string.Empty;

    public RecordStatus Status { get; set; }

    public string StatusLabel { get; set; } = string.Empty;

    public string? FieldBookRef { get; set; }

    public string FacilityName { get; set; } = string.Empty;

    public string LineName { get; set; } = string.Empty;

    public string MachineLabel { get; set; } = string.Empty;

    public string CleaningTypeName { get; set; } = string.Empty;

    public int VersionNo { get; set; }

    public string OwnerName { get; set; } = string.Empty;

    public DateTime OpenedAtLocal { get; set; }

    public DateTime? CompletedAtLocal { get; set; }

    public int? NetSeconds { get; set; }

    public bool HasBelowMinimum { get; set; }

    public bool HasExpiredMaterial { get; set; }

    public string Url { get; set; } = string.Empty;
}
