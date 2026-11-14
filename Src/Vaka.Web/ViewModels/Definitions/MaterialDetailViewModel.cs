namespace Vaka.Web.ViewModels.Definitions;

public sealed class MaterialDetailViewModel
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public MaterialLotItemViewModel[] Lots { get; set; } = [];

    public string? StatusMessage { get; set; }
}
