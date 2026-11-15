using Vaka.Web.Services;
using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.ViewModels.Procedures;

public sealed class CleaningTypeFormViewModel
{
    public int? Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public CleaningTypeKind? Kind { get; set; }

    public string? KindLabel { get; set; }

    public SelectOptionViewModel[] KindOptions { get; set; } = [];

    public bool KindLocked { get; set; }

    public int? DefaultLockMinutes { get; set; }

    public int MaxLockMinutes { get; set; }

    public bool IsActive { get; set; } = true;

    public BlockingRecordViewModel[] BlockingRecords { get; set; } = [];
}
