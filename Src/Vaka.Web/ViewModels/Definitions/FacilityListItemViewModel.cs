namespace Vaka.Web.ViewModels.Definitions;

public sealed class FacilityListItemViewModel
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public int LineCount { get; set; }

    public int ShiftCount { get; set; }
}
