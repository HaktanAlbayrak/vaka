using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.ViewModels.Definitions;
using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed class RecordOptions : IRecordOptions
{
    private readonly VakaDbContext _db;
    private readonly IRecordTimeouts _timeouts;
    private readonly ITurkeyClock _clock;

    public RecordOptions(VakaDbContext db, IRecordTimeouts timeouts, ITurkeyClock clock)
    {
        _db = db;
        _timeouts = timeouts;
        _clock = clock;
    }

    public async Task<SelectOptionViewModel[]> ListFacilitiesAsync(CancellationToken cancellationToken) =>
        await _db.Facilities
            .AsNoTracking()
            .Where(facility => facility.IsActive)
            .OrderBy(facility => facility.Code)
            .Select(facility => new SelectOptionViewModel { Id = facility.Id, Label = facility.Code + " - " + facility.Name })
            .ToArrayAsync(cancellationToken);

    public async Task<SelectOptionViewModel[]> ListLinesAsync(int facilityId, CancellationToken cancellationToken) =>
        await _db.ProductionLines
            .AsNoTracking()
            .Where(line => line.FacilityId == facilityId && line.IsActive && line.Facility.IsActive)
            .OrderBy(line => line.Code)
            .Select(line => new SelectOptionViewModel { Id = line.Id, Label = line.Code + " - " + line.Name })
            .ToArrayAsync(cancellationToken);

    public async Task<MachineOptionViewModel[]> ListMachinesAsync(int lineId, CancellationToken cancellationToken)
    {
        await _timeouts.ApplyAsync(TimeoutScope.Everything, cancellationToken);

        var machines = await _db.Machines
            .AsNoTracking()
            .Where(machine => machine.ProductionLineId == lineId && machine.IsActive && machine.RetiredAtUtc == null
                && machine.Line.IsActive && machine.Line.Facility.IsActive)
            .OrderBy(machine => machine.Code)
            .Select(machine => new { machine.Id, machine.Code, machine.Name })
            .ToListAsync(cancellationToken);

        var ids = machines.Select(machine => machine.Id).ToArray();
        var busy = await _db.CleaningRecords
            .AsNoTracking()
            .Where(record => ids.Contains(record.MachineId)
                && (record.Status == RecordStatus.Opened || record.Status == RecordStatus.InProgress || record.Status == RecordStatus.OnHold))
            .Select(record => new { record.MachineId, OwnerName = record.Owner.FullName, record.Status, record.OpenedAtUtc })
            .ToListAsync(cancellationToken);

        return [.. machines.Select(machine =>
        {
            var record = busy.FirstOrDefault(item => item.MachineId == machine.Id);
            return new MachineOptionViewModel
            {
                Id = machine.Id,
                Label = machine.Code + " - " + machine.Name,
                IsAvailable = record is null,
                BusyReason = record is null
                    ? null
                    : RecordLabels.ActiveRecordSummary(record.OwnerName, record.Status, _clock.ToLocal(record.OpenedAtUtc))
            };
        })];
    }

    public async Task<CleaningTypeOptionViewModel[]> ListCleaningTypesAsync(int machineId, CancellationToken cancellationToken) =>
        await _db.ProcedureVersions
            .AsNoTracking()
            .Where(version => version.Status == ProcedureVersionStatus.Current
                && version.CleaningType.IsActive
                && version.CleaningType.Kind == CleaningTypeKind.Planned
                && version.CleaningType.MachineLinks.Any(link => link.MachineId == machineId))
            .OrderBy(version => version.CleaningType.Name)
            .Select(version => new CleaningTypeOptionViewModel
            {
                Id = version.CleaningTypeId,
                Name = version.CleaningType.Name,
                VersionNo = version.VersionNo,
                MaterialRequired = version.MaterialRequired,
                DefaultLockMinutes = version.CleaningType.DefaultLockMinutes ?? 0
            })
            .ToArrayAsync(cancellationToken);
}
