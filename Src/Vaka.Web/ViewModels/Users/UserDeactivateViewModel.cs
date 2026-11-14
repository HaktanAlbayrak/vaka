namespace Vaka.Web.ViewModels.Users;

public sealed class UserDeactivateViewModel
{
    public int UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string RegistryNo { get; set; } = string.Empty;

    public OpenWorkItemViewModel[] OpenWork { get; set; } = [];

    public bool CanDeactivate { get; set; }

    public string? BlockReason { get; set; }

    public string? Pin { get; set; }
}
