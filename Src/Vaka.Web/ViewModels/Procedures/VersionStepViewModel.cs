namespace Vaka.Web.ViewModels.Procedures;

public sealed class VersionStepViewModel
{
    public int OrderNo { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public StepMediaViewModel[] Media { get; set; } = [];
}
