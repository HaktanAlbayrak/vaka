namespace Vaka.Web.ViewModels.Dashboard;

public sealed class DashboardDayCountViewModel
{
    public DateOnly Date { get; set; }

    public string DateLabel { get; set; } = string.Empty;

    public int PlannedCount { get; set; }

    public int EmergencyCount { get; set; }
}
