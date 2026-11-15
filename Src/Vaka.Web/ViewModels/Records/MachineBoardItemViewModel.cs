using Vaka.Web.Services;

namespace Vaka.Web.ViewModels.Records;

public sealed class MachineBoardItemViewModel
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public MachineState State { get; set; }

    public string StateLabel { get; set; } = string.Empty;

    public int? RecordId { get; set; }

    public string? RecordNo { get; set; }

    public string? OwnerName { get; set; }

    public DateTime? OpenedAtLocal { get; set; }

    public DateTime? LockExpiresAtLocal { get; set; }

    public string? RecordUrl { get; set; }
}
