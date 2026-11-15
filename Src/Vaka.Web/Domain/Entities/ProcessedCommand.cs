namespace Vaka.Web.Domain.Entities;

public class ProcessedCommand
{
    public string CommandId { get; set; } = string.Empty;
    public string CommandType { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string ResultJson { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}
