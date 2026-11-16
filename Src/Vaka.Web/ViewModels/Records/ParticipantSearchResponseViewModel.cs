namespace Vaka.Web.ViewModels.Records;

public sealed class ParticipantSearchResponseViewModel
{
    public ParticipantOptionViewModel[] Items { get; set; } = [];

    public bool Truncated { get; set; }
}
