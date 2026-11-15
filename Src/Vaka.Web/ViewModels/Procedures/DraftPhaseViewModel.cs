namespace Vaka.Web.ViewModels.Procedures;

public sealed class DraftPhaseViewModel
{
    public int Id { get; set; }

    public int OrderNo { get; set; }

    public string Title { get; set; } = string.Empty;

    public int? MinDurationMinutes { get; set; }

    public bool CanMoveUp { get; set; }

    public bool CanMoveDown { get; set; }

    public DraftStepViewModel[] Steps { get; set; } = [];
}
