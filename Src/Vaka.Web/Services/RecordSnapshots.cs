using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

public static class RecordSnapshots
{
    /// <summary>Tesis, hat ve makine kod ve adlarını kayda açılış anındaki haliyle yazar; sonraki yeniden adlandırma kaydı değiştirmez.</summary>
    public static void ApplyMachine(this CleaningRecord record, PlannedMachine machine)
    {
        record.MachineId = machine.Id;
        record.FacilityCodeSnapshot = machine.FacilityCode;
        record.FacilityNameSnapshot = machine.FacilityName;
        record.LineCodeSnapshot = machine.LineCode;
        record.LineNameSnapshot = machine.LineName;
        record.MachineCodeSnapshot = machine.Code;
        record.MachineNameSnapshot = machine.Name;
    }

    public static RecordMaterial ToRecordMaterial(this PlannedLot lot, int enteredByUserId, DateTime enteredAtUtc) =>
        new()
        {
            MaterialLotId = lot.LotId,
            MaterialCodeSnapshot = lot.MaterialCode,
            MaterialNameSnapshot = lot.MaterialName,
            LotNoSnapshot = lot.LotNo,
            ExpiryDateSnapshot = lot.ExpiryDate,
            ExpiredAtEntry = lot.IsExpired,
            ExpiredReason = lot.ExpiredReason,
            EnteredByUserId = enteredByUserId,
            EnteredAtUtc = enteredAtUtc
        };
}
