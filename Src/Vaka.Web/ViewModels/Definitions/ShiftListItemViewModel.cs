namespace Vaka.Web.ViewModels.Definitions;

public sealed class ShiftListItemViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool EndsNextDay { get; set; }

    public bool IsActive { get; set; }
}
