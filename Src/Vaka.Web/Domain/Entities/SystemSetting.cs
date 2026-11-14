namespace Vaka.Web.Domain.Entities;

public class SystemSetting
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public int Value { get; set; }
    public string Unit { get; set; } = string.Empty;
}
