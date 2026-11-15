namespace Vaka.Web.ViewModels.Dashboard;

public sealed class DashboardViewModel
{
    public int OpenCount { get; set; }

    public int InProgressCount { get; set; }

    public int CompletedTodayCount { get; set; }

    public DashboardActiveRecordViewModel[] ActiveRecords { get; set; } = [];

    public DashboardBelowMinimumViewModel[] BelowMinimum { get; set; } = [];

    public DashboardCompletedViewModel[] RecentCompleted { get; set; } = [];

    public DashboardDayCountViewModel[] Last7Days { get; set; } = [];
}
