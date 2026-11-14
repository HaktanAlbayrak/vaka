namespace Vaka.Web.ViewModels.Definitions;

public sealed class ShiftListViewModel
{
    public int FacilityId { get; set; }

    public FacilityOptionViewModel[] Facilities { get; set; } = [];

    public ShiftListItemViewModel[] Items { get; set; } = [];

    public string? StatusMessage { get; set; }
}
