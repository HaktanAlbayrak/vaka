namespace Vaka.Web.Domain.Entities;

public class CleaningEvent
{
    public int Id { get; set; }
    public int CleaningRecordId { get; set; }
    public int Sequence { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = string.Empty;
    public int? ActorUserId { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    public string? CommandId { get; set; }
    public string? PreviousHash { get; set; }
    public string Hash { get; set; } = string.Empty;

    public CleaningRecord Record { get; set; } = null!;
    public User? Actor { get; set; }
}
