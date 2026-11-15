namespace Vaka.Web.ViewModels.Procedures;

public sealed class MachineMappingGroupViewModel
{
    public string FacilityLabel { get; set; } = string.Empty;

    public string LineLabel { get; set; } = string.Empty;

    public MachineMappingItemViewModel[] Machines { get; set; } = [];
}
