namespace Vaka.Web.ViewModels.Definitions;

public sealed class FacilityLineItemViewModel
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public int MachineCount { get; set; }
}
