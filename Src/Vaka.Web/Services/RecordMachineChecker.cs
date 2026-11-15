using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;

namespace Vaka.Web.Services;

public sealed class RecordMachineChecker : IRecordMachineChecker
{
    private readonly VakaDbContext _db;

    public RecordMachineChecker(VakaDbContext db)
    {
        _db = db;
    }

    public async Task<PlannedMachine?> CheckAsync(int? facilityId, int? lineId, int? machineId, List<FieldError> errors, CancellationToken cancellationToken)
    {
        var machine = machineId is null
            ? null
            : await _db.Machines
                .AsNoTracking()
                .Where(item => item.Id == machineId)
                .Select(item => new
                {
                    Row = new PlannedMachine(
                        item.Id, item.Code, item.Name, item.Line.Id, item.Line.Code, item.Line.Name,
                        item.Line.FacilityId, item.Line.Facility.Code, item.Line.Facility.Name),
                    item.IsActive,
                    item.RetiredAtUtc,
                    LineActive = item.Line.IsActive,
                    FacilityActive = item.Line.Facility.IsActive
                })
                .FirstOrDefaultAsync(cancellationToken);

        if (machine is null)
        {
            errors.Add(new FieldError("MachineId", "Makine seçin."));
            return null;
        }

        var before = errors.Count;
        if (facilityId != machine.Row.FacilityId || !machine.FacilityActive)
        {
            errors.Add(new FieldError("FacilityId", "Tesis seçilen makineye uygun değil ya da aktif değil."));
        }

        if (lineId != machine.Row.LineId || !machine.LineActive)
        {
            errors.Add(new FieldError("LineId", "Hat seçilen makineye uygun değil ya da aktif değil."));
        }

        if (!machine.IsActive || machine.RetiredAtUtc is not null)
        {
            errors.Add(new FieldError("MachineId", "Makine kullanımda değil."));
        }

        return errors.Count == before ? machine.Row : null;
    }
}
