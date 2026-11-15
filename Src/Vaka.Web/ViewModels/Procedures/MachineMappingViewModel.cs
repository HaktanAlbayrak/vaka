namespace Vaka.Web.ViewModels.Procedures;

public sealed class MachineMappingViewModel
{
    public int CleaningTypeId { get; set; }

    public string CleaningTypeName { get; set; } = string.Empty;

    public MachineMappingGroupViewModel[] Groups { get; set; } = [];

    public int[] SelectedMachineIds { get; set; } = [];
}
