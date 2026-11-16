namespace Vaka.Web.ViewModels.Reports;

public sealed class MachineRecentRecordViewModel
{
    public int RecordId { get; set; }

    public string RecordNo { get; set; } = string.Empty;

    public string CleaningTypeName { get; set; } = string.Empty;

    public string KindLabel { get; set; } = string.Empty;

    public DateTime OpenedAtLocal { get; set; }

    public string StatusLabel { get; set; } = string.Empty;

    public int? NetSeconds { get; set; }

    public int? EffortSeconds { get; set; }

    public int? PlannedMinSeconds { get; set; }

    public string Url { get; set; } = string.Empty;
}
