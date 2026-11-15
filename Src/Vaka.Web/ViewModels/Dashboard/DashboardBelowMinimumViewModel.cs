namespace Vaka.Web.ViewModels.Dashboard;

public sealed class DashboardBelowMinimumViewModel
{
    public int RecordId { get; set; }

    public string RecordNo { get; set; } = string.Empty;

    public string MachineLabel { get; set; } = string.Empty;

    public string PhaseTitle { get; set; } = string.Empty;

    public int NetSeconds { get; set; }

    public int MinDurationSeconds { get; set; }

    public string Reason { get; set; } = string.Empty;

    public DateTime CompletedAtLocal { get; set; }

    public string Url { get; set; } = string.Empty;
}
