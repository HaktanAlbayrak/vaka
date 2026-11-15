namespace Vaka.Web.ViewModels.Procedures;

public sealed class VersionPhaseViewModel
{
    public int OrderNo { get; set; }

    public string Title { get; set; } = string.Empty;

    public int? MinDurationMinutes { get; set; }

    public VersionStepViewModel[] Steps { get; set; } = [];
}
