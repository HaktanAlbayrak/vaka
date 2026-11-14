using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Vaka.Web.ViewModels.Users;

public sealed class UserFormViewModel
{
    public int? Id { get; set; }

    public string RegistryNo { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string[] SelectedRoles { get; set; } = [];

    public string[] AvailableRoles { get; set; } = [];

    public bool RolesLocked { get; set; }

    public string? TemporaryPassword { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsPinLocked { get; set; }

    public bool MustChangePassword { get; set; }

    public bool MustSetPin { get; set; }

    public bool CanResetPin { get; set; }

    public bool CanResetPassword { get; set; }

    public bool CanDeactivate { get; set; }

    public bool CanActivate { get; set; }

    public string? DeactivateBlockReason { get; set; }

    public string? StatusMessage { get; set; }

    [ValidateNever]
    public UserResetViewModel ResetPin { get; set; } = new();

    [ValidateNever]
    public UserResetViewModel ResetPassword { get; set; } = new();

    [ValidateNever]
    public UserResetViewModel Activate { get; set; } = new();
}
