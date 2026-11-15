namespace Vaka.Web.ViewModels.Records;

public sealed class RecordIntegrityViewModel
{
    public bool IsValid { get; set; }

    public int? FirstBrokenSequence { get; set; }

    public DateTime CheckedAtLocal { get; set; }

    /// <summary>Yalnız doğrulama ekranını açabilen Yönetici ve Denetçi'de dolu.</summary>
    public string? ReportUrl { get; set; }
}
