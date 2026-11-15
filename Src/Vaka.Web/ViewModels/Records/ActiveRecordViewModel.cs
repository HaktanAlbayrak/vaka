using Vaka.Web.Services;

namespace Vaka.Web.ViewModels.Records;

public sealed class ActiveRecordViewModel
{
    public int Id { get; set; }

    public string RecordNo { get; set; } = string.Empty;

    public CleaningTypeKind Kind { get; set; }

    public string KindLabel { get; set; } = string.Empty;

    public RecordStatus Status { get; set; }

    public string StatusLabel { get; set; } = string.Empty;

    public string MachineLabel { get; set; } = string.Empty;

    public string CleaningTypeName { get; set; } = string.Empty;

    public int VersionNo { get; set; }

    public DateTime? LockExpiresAtLocal { get; set; }

    public string Url { get; set; } = string.Empty;
}
