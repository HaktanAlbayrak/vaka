namespace Vaka.Web.ViewModels.Records;

public sealed class RecordTimelineDayViewModel
{
    public DateOnly DateLocal { get; set; }

    public RecordTimelineEntryViewModel[] Entries { get; set; } = [];
}
