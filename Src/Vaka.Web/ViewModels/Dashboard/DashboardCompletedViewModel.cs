using Vaka.Web.Services;

namespace Vaka.Web.ViewModels.Dashboard;

public sealed class DashboardCompletedViewModel
{
    public int RecordId { get; set; }

    public string RecordNo { get; set; } = string.Empty;

    public CleaningTypeKind Kind { get; set; }

    public string KindLabel { get; set; } = string.Empty;

    public string MachineLabel { get; set; } = string.Empty;

    public DateTime CompletedAtLocal { get; set; }

    public int NetSeconds { get; set; }

    public int EffortSeconds { get; set; }

    public string Url { get; set; } = string.Empty;
}
