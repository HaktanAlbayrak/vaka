using Vaka.Web.Services;

namespace Vaka.Web.ViewModels.Procedures;

public sealed class StepMediaViewModel
{
    public int Id { get; set; }

    public StepMediaType MediaType { get; set; }

    public string MediaTypeLabel { get; set; } = string.Empty;

    public string OriginalFileName { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;
}
