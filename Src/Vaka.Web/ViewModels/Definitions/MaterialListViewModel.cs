namespace Vaka.Web.ViewModels.Definitions;

public sealed class MaterialListViewModel
{
    public MaterialListItemViewModel[] Items { get; set; } = [];

    public string? Query { get; set; }

    public bool ShowInactive { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; }

    public int Total { get; set; }

    public string? StatusMessage { get; set; }
}
