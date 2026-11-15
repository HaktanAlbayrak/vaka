using Vaka.Web.Services;
using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.ViewModels.Procedures;

public sealed class CleaningTypeListViewModel
{
    public CleaningTypeListItemViewModel[] Items { get; set; } = [];

    public CleaningTypeKind? Kind { get; set; }

    public SelectOptionViewModel[] KindOptions { get; set; } = [];

    public bool ShowInactive { get; set; }

    public string? StatusMessage { get; set; }
}
