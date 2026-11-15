namespace Vaka.Web.ViewModels.Records;

public sealed class IntegrityEventViewModel
{
    public int Sequence { get; set; }

    public string EventLabel { get; set; } = string.Empty;

    public DateTime OccurredAtLocal { get; set; }

    public string HashShort { get; set; } = string.Empty;

    public bool IsValid { get; set; }

    public string? Problem { get; set; }
}
