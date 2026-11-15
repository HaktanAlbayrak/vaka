namespace Vaka.Web.ViewModels.Records;

public sealed class MachineBoardGroupViewModel
{
    public string FacilityName { get; set; } = string.Empty;

    public string LineName { get; set; } = string.Empty;

    public MachineBoardItemViewModel[] Machines { get; set; } = [];
}
