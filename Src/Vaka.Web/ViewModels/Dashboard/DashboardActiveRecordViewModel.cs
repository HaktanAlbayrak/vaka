using Vaka.Web.Services;

namespace Vaka.Web.ViewModels.Dashboard;

public sealed class DashboardActiveRecordViewModel
{
    public int RecordId { get; set; }

    public string RecordNo { get; set; } = string.Empty;

    public CleaningTypeKind Kind { get; set; }

    public string KindLabel { get; set; } = string.Empty;

    public RecordStatus Status { get; set; }

    public string StatusLabel { get; set; } = string.Empty;

    public string MachineLabel { get; set; } = string.Empty;

    public string OwnerName { get; set; } = string.Empty;

    public DateTime OpenedAtLocal { get; set; }

    public DateTime? LockExpiresAtLocal { get; set; }

    public string? CurrentStepTitle { get; set; }

    public string Url { get; set; } = string.Empty;
}
