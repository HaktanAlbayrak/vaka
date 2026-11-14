namespace Vaka.Web.Domain.Entities;

public class SettingChange
{
    public int Id { get; set; }
    public int SettingId { get; set; }
    public int OldValue { get; set; }
    public int NewValue { get; set; }
    public int ChangedByUserId { get; set; }
    public DateTime ChangedAtUtc { get; set; }

    public SystemSetting Setting { get; set; } = null!;
    public User ChangedByUser { get; set; } = null!;
}
