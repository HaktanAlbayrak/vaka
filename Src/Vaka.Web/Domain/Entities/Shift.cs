namespace Vaka.Web.Domain.Entities;

public class Shift
{
    public int Id { get; set; }
    public int FacilityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsActive { get; set; } = true;

    public Facility Facility { get; set; } = null!;
}
