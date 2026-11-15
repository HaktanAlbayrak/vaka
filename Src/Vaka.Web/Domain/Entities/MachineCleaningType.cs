namespace Vaka.Web.Domain.Entities;

public class MachineCleaningType
{
    public int MachineId { get; set; }
    public int CleaningTypeId { get; set; }

    public Machine Machine { get; set; } = null!;
    public CleaningType CleaningType { get; set; } = null!;
}
