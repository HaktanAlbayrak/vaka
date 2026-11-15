using Vaka.Web.Services;

namespace Vaka.Web.ViewModels.Procedures;

public sealed class CleaningTypeListItemViewModel
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public CleaningTypeKind Kind { get; set; }

    public string KindLabel { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public int? CurrentVersionNo { get; set; }

    public int? DraftVersionNo { get; set; }

    public int MachineCount { get; set; }
}
