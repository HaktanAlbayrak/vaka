namespace Vaka.Web.ViewModels.Procedures;

public sealed class DraftFormErrorViewModel
{
    public string Target { get; set; } = string.Empty;

    public string[] Messages { get; set; } = [];

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int? MinDurationMinutes { get; set; }

    public string? DocumentRef { get; set; }

    public bool? MaterialRequired { get; set; }
}
