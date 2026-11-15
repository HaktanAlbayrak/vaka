namespace Vaka.Web.ViewModels.Procedures;

public sealed class MachineMappingItemViewModel
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public bool IsSelected { get; set; }

    public bool IsSelectable { get; set; }

    public string? NotSelectableReason { get; set; }
}
