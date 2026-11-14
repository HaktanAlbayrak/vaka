namespace Vaka.Web.ViewModels.Users;

public sealed class UserListItemViewModel
{
    public int Id { get; set; }

    public string RegistryNo { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string[] Roles { get; set; } = [];

    public bool IsActive { get; set; }

    public bool IsPinLocked { get; set; }

    public bool MustChangePassword { get; set; }

    public bool MustSetPin { get; set; }

    public bool CanManage { get; set; }
}
