namespace Vaka.Web.ViewModels.Definitions;

public sealed class MachineListViewModel
{
    public MachineListItemViewModel[] Items { get; set; } = [];

    public int? FacilityId { get; set; }

    public int? LineId { get; set; }

    public string? Query { get; set; }

    public bool ShowRetired { get; set; }

    public SelectOptionViewModel[] Facilities { get; set; } = [];

    public SelectOptionViewModel[] Lines { get; set; } = [];

    public int Page { get; set; } = 1;

    public int PageSize { get; set; }

    public int Total { get; set; }

    public string? StatusMessage { get; set; }
}
