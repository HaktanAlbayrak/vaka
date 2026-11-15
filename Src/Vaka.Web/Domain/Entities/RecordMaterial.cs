namespace Vaka.Web.Domain.Entities;

public class RecordMaterial
{
    public int Id { get; set; }
    public int CleaningRecordId { get; set; }
    public int MaterialLotId { get; set; }
    public string MaterialCodeSnapshot { get; set; } = string.Empty;
    public string MaterialNameSnapshot { get; set; } = string.Empty;
    public string LotNoSnapshot { get; set; } = string.Empty;
    public DateOnly ExpiryDateSnapshot { get; set; }
    public bool ExpiredAtEntry { get; set; }
    public string? ExpiredReason { get; set; }
    public int EnteredByUserId { get; set; }
    public DateTime EnteredAtUtc { get; set; }
    public DateTime? RemovedAtUtc { get; set; }

    public CleaningRecord Record { get; set; } = null!;
    public MaterialLot MaterialLot { get; set; } = null!;
    public User EnteredBy { get; set; } = null!;
}
