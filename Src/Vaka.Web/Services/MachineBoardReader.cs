using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed class MachineBoardReader : IMachineBoardReader
{
    private readonly VakaDbContext _db;
    private readonly IRecordTimeouts _timeouts;
    private readonly IRecordOptions _options;
    private readonly ITurkeyClock _clock;
    private readonly TimeProvider _time;

    public MachineBoardReader(VakaDbContext db, IRecordTimeouts timeouts, IRecordOptions options, ITurkeyClock clock, TimeProvider time)
    {
        _db = db;
        _timeouts = timeouts;
        _options = options;
        _clock = clock;
        _time = time;
    }

    public async Task<MachineBoardViewModel> GetAsync(int? facilityId, IReadOnlyCollection<string> viewerRoles, CancellationToken cancellationToken)
    {
        await _timeouts.ApplyAsync(TimeoutScope.Everything, cancellationToken);

        var facilities = await _options.ListFacilitiesAsync(cancellationToken);
        var selected = facilityId is { } id && facilities.Any(item => item.Id == id) ? id : (int?)null;

        var machines = await _db.Machines
            .AsNoTracking()
            .Where(machine => machine.IsActive && machine.RetiredAtUtc == null && machine.Line.IsActive && machine.Line.Facility.IsActive
                && (selected == null || machine.Line.FacilityId == selected))
            .OrderBy(machine => machine.Line.Facility.Code).ThenBy(machine => machine.Line.Code).ThenBy(machine => machine.Code)
            .Select(machine => new
            {
                machine.Id,
                machine.Code,
                machine.Name,
                FacilityName = machine.Line.Facility.Name,
                LineName = machine.Line.Name
            })
            .ToListAsync(cancellationToken);

        var ids = machines.Select(machine => machine.Id).ToArray();
        var records = (await _db.CleaningRecords
            .AsNoTracking()
            .Where(record => ids.Contains(record.MachineId)
                && (record.Status == RecordStatus.Opened || record.Status == RecordStatus.InProgress || record.Status == RecordStatus.OnHold))
            .Select(record => new
            {
                record.Id,
                record.MachineId,
                record.RecordNo,
                record.Status,
                OwnerName = record.Owner.FullName,
                record.OpenedAtUtc,
                record.LockExpiresAtUtc
            })
            .ToListAsync(cancellationToken)).ToDictionary(record => record.MachineId);

        var groups = machines
            .GroupBy(machine => (machine.FacilityName, machine.LineName))
            .Select(group => new MachineBoardGroupViewModel
            {
                FacilityName = group.Key.FacilityName,
                LineName = group.Key.LineName,
                Machines = [.. group.Select(machine => ToItem(machine.Id, machine.Code, machine.Name, records.GetValueOrDefault(machine.Id) is { } record
                    ? (record.Id, record.RecordNo, record.Status, record.OwnerName, record.OpenedAtUtc, record.LockExpiresAtUtc)
                    : null, viewerRoles))]
            })
            .ToArray();

        return new MachineBoardViewModel
        {
            FacilityId = selected,
            Facilities = facilities,
            Groups = groups,
            GeneratedAtLocal = _clock.ToLocal(_time.GetUtcNow().UtcDateTime)
        };
    }

    private MachineBoardItemViewModel ToItem(
        int id,
        string code,
        string name,
        (int Id, string RecordNo, RecordStatus Status, string OwnerName, DateTime OpenedAtUtc, DateTime? LockExpiresAtUtc)? record,
        IReadOnlyCollection<string> viewerRoles)
    {
        if (record is not { } active)
        {
            return new MachineBoardItemViewModel { Id = id, Code = code, Name = name, State = MachineState.Free, StateLabel = RecordLabels.Machine(MachineState.Free) };
        }

        var state = active.Status switch
        {
            RecordStatus.Opened => MachineState.Opened,
            RecordStatus.InProgress => MachineState.InProgress,
            _ => MachineState.OnHold
        };

        return new MachineBoardItemViewModel
        {
            Id = id,
            Code = code,
            Name = name,
            State = state,
            StateLabel = RecordLabels.Machine(state),
            RecordId = active.Id,
            RecordNo = active.RecordNo,
            OwnerName = active.OwnerName,
            OpenedAtLocal = _clock.ToLocal(active.OpenedAtUtc),
            LockExpiresAtLocal = state == MachineState.Opened && active.LockExpiresAtUtc is { } expires ? _clock.ToLocal(expires) : null,
            RecordUrl = RecordLinks.For(viewerRoles, active.Id)
        };
    }
}
