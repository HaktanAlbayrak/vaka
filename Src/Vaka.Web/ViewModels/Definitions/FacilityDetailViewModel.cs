namespace Vaka.Web.ViewModels.Definitions;

public sealed class FacilityDetailViewModel
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public FacilityLineItemViewModel[] Lines { get; set; } = [];

    public string? StatusMessage { get; set; }
}
