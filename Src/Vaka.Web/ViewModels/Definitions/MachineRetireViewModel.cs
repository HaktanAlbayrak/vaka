namespace Vaka.Web.ViewModels.Definitions;

public sealed class MachineRetireViewModel
{
    public int MachineId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public BlockingRecordViewModel[] BlockingRecords { get; set; } = [];

    public bool CanRetire { get; set; }

    public DateTime? RetiredAtLocal { get; set; }
}
