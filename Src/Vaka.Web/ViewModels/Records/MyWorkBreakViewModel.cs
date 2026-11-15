namespace Vaka.Web.ViewModels.Records;

public sealed class MyWorkBreakViewModel
{
    public int BreakId { get; set; }

    public string RecordNo { get; set; } = string.Empty;

    public DateTime StartedAtLocal { get; set; }
}
