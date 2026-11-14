namespace Vaka.Web.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string RegistryNo { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? PinHash { get; set; }
    public bool MustChangePassword { get; set; }
    public bool MustSetPin { get; set; }
    public int PinFailedCount { get; set; }
    public DateTime? PinLockedAtUtc { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; }

    public ICollection<UserRole> Roles { get; set; } = new List<UserRole>();
}
