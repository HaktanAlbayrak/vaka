namespace Vaka.Web.Domain.Entities;

public class UserRole
{
    public int UserId { get; set; }
    public string Role { get; set; } = string.Empty;

    public User User { get; set; } = null!;
}
