namespace Vaka.Web.Domain.Entities;

public class StepParticipant
{
    public int Id { get; set; }
    public int RecordStepId { get; set; }
    public int UserId { get; set; }
    public bool IsWorking { get; set; }
    public int AddedByUserId { get; set; }
    public DateTime? ConfirmedAtUtc { get; set; }

    public RecordStep Step { get; set; } = null!;
    public User User { get; set; } = null!;
    public User AddedBy { get; set; } = null!;
}
