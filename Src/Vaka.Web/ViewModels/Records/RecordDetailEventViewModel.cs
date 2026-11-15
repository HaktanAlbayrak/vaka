namespace Vaka.Web.ViewModels.Records;

public sealed class RecordDetailEventViewModel
{
    public int Sequence { get; set; }

    public DateTime OccurredAtLocal { get; set; }

    public string ActorName { get; set; } = string.Empty;

    public string EventLabel { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;
}
