using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;
using Vaka.Web.ViewModels.Procedures;

namespace Vaka.Web.Services;

public sealed class MachineMappingService : IMachineMappingService
{
    private readonly VakaDbContext _db;

    public MachineMappingService(VakaDbContext db)
    {
        _db = db;
    }

    public async Task<LookupResult<MachineMappingViewModel>> GetAsync(int cleaningTypeId, CancellationToken cancellationToken)
    {
        var type = await _db.CleaningTypes
            .AsNoTracking()
            .Where(item => item.Id == cleaningTypeId)
            .Select(item => new { item.Name, item.Kind })
            .FirstOrDefaultAsync(cancellationToken);

        if (type is null)
        {
            return LookupResult<MachineMappingViewModel>.Missing();
        }

        if (type.Kind != CleaningTypeKind.Planned)
        {
            return LookupResult<MachineMappingViewModel>.Refused();
        }

        var machines = await ReadMachinesAsync(cleaningTypeId, cancellationToken);
        var groups = machines
            .GroupBy(machine => new { machine.FacilityCode, machine.FacilityName, machine.LineCode, machine.LineName })
            .Select(group => new MachineMappingGroupViewModel
            {
                FacilityLabel = $"{group.Key.FacilityCode} - {group.Key.FacilityName}",
                LineLabel = $"{group.Key.LineCode} - {group.Key.LineName}",
                Machines = group.Select(machine => new MachineMappingItemViewModel
                {
                    Id = machine.Id,
                    Code = machine.Code,
                    Name = machine.Name,
                    IsSelected = machine.IsLinked,
                    IsSelectable = machine.Reason is null,
                    NotSelectableReason = machine.Reason
                }).ToArray()
            }).ToArray();

        return LookupResult<MachineMappingViewModel>.Found(new MachineMappingViewModel
        {
            CleaningTypeId = cleaningTypeId,
            CleaningTypeName = type.Name,
            Groups = groups,
            SelectedMachineIds = machines.Where(machine => machine.IsLinked).Select(machine => machine.Id).ToArray()
        });
    }

    public async Task<CommandResult> SaveAsync(int cleaningTypeId, IReadOnlyCollection<int> selectedMachineIds, CancellationToken cancellationToken)
    {
        var kind = await _db.CleaningTypes
            .AsNoTracking()
            .Where(item => item.Id == cleaningTypeId)
            .Select(item => (CleaningTypeKind?)item.Kind)
            .FirstOrDefaultAsync(cancellationToken);

        if (kind is null)
        {
            return CommandResult.NotFound();
        }

        if (kind != CleaningTypeKind.Planned)
        {
            return CommandResult.Rejected(string.Empty, "Acil tipe makine eşlenmez.");
        }

        var machines = await ReadMachinesAsync(cleaningTypeId, cancellationToken);
        var selectable = machines.Where(machine => machine.Reason is null).ToList();
        var selected = selectedMachineIds.ToHashSet();

        var toRemove = selectable.Where(machine => machine.IsLinked && !selected.Contains(machine.Id)).Select(machine => machine.Id).ToArray();
        var toAdd = selectable.Where(machine => !machine.IsLinked && selected.Contains(machine.Id)).Select(machine => machine.Id).ToArray();

        if (toRemove.Length > 0)
        {
            var links = await _db.MachineCleaningTypes
                .Where(link => link.CleaningTypeId == cleaningTypeId && toRemove.Contains(link.MachineId))
                .ToListAsync(cancellationToken);
            _db.MachineCleaningTypes.RemoveRange(links);
        }

        foreach (var machineId in toAdd)
        {
            _db.MachineCleaningTypes.Add(new MachineCleaningType { MachineId = machineId, CleaningTypeId = cleaningTypeId });
        }

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return CommandResult.Rejected(string.Empty, "Eşleme kaydedilemedi. Sayfayı yenileyip yeniden deneyin.");
        }

        return CommandResult.Success(cleaningTypeId);
    }

    private async Task<List<MachineRow>> ReadMachinesAsync(int cleaningTypeId, CancellationToken cancellationToken)
    {
        var rows = await _db.Machines
            .AsNoTracking()
            .OrderBy(machine => machine.Line.Facility.Code)
            .ThenBy(machine => machine.Line.Code)
            .ThenBy(machine => machine.Code)
            .Select(machine => new
            {
                machine.Id,
                machine.Code,
                machine.Name,
                machine.IsActive,
                machine.RetiredAtUtc,
                LineCode = machine.Line.Code,
                LineName = machine.Line.Name,
                LineActive = machine.Line.IsActive,
                FacilityCode = machine.Line.Facility.Code,
                FacilityName = machine.Line.Facility.Name,
                FacilityActive = machine.Line.Facility.IsActive,
                IsLinked = machine.CleaningTypeLinks.Any(link => link.CleaningTypeId == cleaningTypeId)
            })
            .ToListAsync(cancellationToken);

        return rows.Select(row => new MachineRow(
            row.Id,
            row.Code,
            row.Name,
            row.FacilityCode,
            row.FacilityName,
            row.LineCode,
            row.LineName,
            row.IsLinked,
            ReasonFor(row.RetiredAtUtc is not null, row.IsActive, row.LineActive, row.FacilityActive))).ToList();
    }

    private static string? ReasonFor(bool retired, bool machineActive, bool lineActive, bool facilityActive)
    {
        if (retired)
        {
            return "Makine kullanımdan kaldırılmış.";
        }

        if (!machineActive)
        {
            return "Makine pasif.";
        }

        if (!lineActive)
        {
            return "Hat pasif.";
        }

        return facilityActive ? null : "Tesis pasif.";
    }

    private sealed record MachineRow(
        int Id,
        string Code,
        string Name,
        string FacilityCode,
        string FacilityName,
        string LineCode,
        string LineName,
        bool IsLinked,
        string? Reason);
}
