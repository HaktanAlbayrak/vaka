namespace Vaka.Web.ViewModels.Records;

public sealed class IntegrityReportViewModel
{
    public int RecordId { get; set; }

    public string RecordNo { get; set; } = string.Empty;

    public bool IsValid { get; set; }

    public DateTime CheckedAtLocal { get; set; }

    public IntegrityEventViewModel[] Events { get; set; } = [];

    public IntegrityMismatchViewModel[] StateMismatches { get; set; } = [];
}
