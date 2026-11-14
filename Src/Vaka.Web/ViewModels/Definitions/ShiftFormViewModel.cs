namespace Vaka.Web.ViewModels.Definitions;

public sealed class ShiftFormViewModel
{
    public int? Id { get; set; }

    public int FacilityId { get; set; }

    public string FacilityName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public bool IsActive { get; set; } = true;
}
