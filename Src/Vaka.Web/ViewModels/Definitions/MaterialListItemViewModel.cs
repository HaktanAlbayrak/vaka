namespace Vaka.Web.ViewModels.Definitions;

public sealed class MaterialListItemViewModel
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public int LotCount { get; set; }

    public int ExpiredLotCount { get; set; }
}
