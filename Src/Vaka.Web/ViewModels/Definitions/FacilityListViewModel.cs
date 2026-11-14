namespace Vaka.Web.ViewModels.Definitions;

public sealed class FacilityListViewModel
{
    public FacilityListItemViewModel[] Items { get; set; } = [];

    public bool ShowInactive { get; set; }

    public string? StatusMessage { get; set; }
}
