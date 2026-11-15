namespace Vaka.Web.ViewModels.Procedures;

public sealed class DraftStepViewModel
{
    public int Id { get; set; }

    public int OrderNo { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool CanMoveUp { get; set; }

    public bool CanMoveDown { get; set; }

    public StepMediaViewModel[] Media { get; set; } = [];
}
