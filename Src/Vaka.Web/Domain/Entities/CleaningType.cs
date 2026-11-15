using Vaka.Web.Services;

namespace Vaka.Web.Domain.Entities;

public class CleaningType
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public CleaningTypeKind Kind { get; set; }
    public int? DefaultLockMinutes { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<MachineCleaningType> MachineLinks { get; set; } = new List<MachineCleaningType>();
    public ICollection<ProcedureVersion> Versions { get; set; } = new List<ProcedureVersion>();
}
