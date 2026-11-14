namespace Vaka.Web.ViewModels.Definitions;

public sealed class LineFormViewModel
{
    public int? Id { get; set; }

    public int FacilityId { get; set; }

    public string FacilityName { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public bool CodeLocked { get; set; }
}
