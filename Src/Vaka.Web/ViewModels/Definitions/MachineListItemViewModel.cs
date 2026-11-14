namespace Vaka.Web.ViewModels.Definitions;

public sealed class MachineListItemViewModel
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string FacilityName { get; set; } = string.Empty;

    public string LineName { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime? RetiredAtLocal { get; set; }
}
