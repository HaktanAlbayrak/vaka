using Vaka.Web.Services;

namespace Vaka.Web.ViewModels.Records;

public sealed class RecordDetailHeaderViewModel
{
    public string RecordNo { get; set; } = string.Empty;

    public CleaningTypeKind Kind { get; set; }

    public string KindLabel { get; set; } = string.Empty;

    public RecordStatus Status { get; set; }

    public string StatusLabel { get; set; } = string.Empty;

    public string? FieldBookRef { get; set; }

    public string Facility { get; set; } = string.Empty;

    public string Line { get; set; } = string.Empty;

    public string Machine { get; set; } = string.Empty;

    public string CleaningTypeName { get; set; } = string.Empty;

    public int VersionNo { get; set; }

    /// <summary>Yalnız prosedür ekranını açabilen Yönetici ve Denetçi'de dolu.</summary>
    public string? VersionUrl { get; set; }

    public string? DocumentRef { get; set; }

    public string WorkOrderNo { get; set; } = string.Empty;

    public string Note { get; set; } = string.Empty;

    public string? EmergencyDescription { get; set; }
}
