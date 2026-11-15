using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Services;

public sealed class ReportOptions : IReportOptions
{
    private readonly VakaDbContext _db;

    public ReportOptions(VakaDbContext db)
    {
        _db = db;
    }

    public async Task<SelectOptionViewModel[]> ListFacilitiesAsync(CancellationToken cancellationToken) =>
        await _db.Facilities
            .AsNoTracking()
            .OrderBy(facility => facility.Code)
            .Select(facility => new SelectOptionViewModel { Id = facility.Id, Label = facility.Code + " - " + facility.Name })
            .ToArrayAsync(cancellationToken);

    public async Task<SelectOptionViewModel[]> ListLinesAsync(int? facilityId, CancellationToken cancellationToken) =>
        facilityId is null ? [] : await _db.ProductionLines
            .AsNoTracking()
            .Where(line => line.FacilityId == facilityId)
            .OrderBy(line => line.Facility.Code).ThenBy(line => line.Code)
            .Select(line => new SelectOptionViewModel { Id = line.Id, Label = line.Code + " - " + line.Name })
            .ToArrayAsync(cancellationToken);

    public async Task<SelectOptionViewModel[]> ListMachinesAsync(int? facilityId, int? lineId, CancellationToken cancellationToken) =>
        facilityId is null && lineId is null ? [] : await _db.Machines
            .AsNoTracking()
            .Where(machine => (lineId == null || machine.ProductionLineId == lineId) && (facilityId == null || machine.Line.FacilityId == facilityId))
            .OrderBy(machine => machine.Line.Facility.Code).ThenBy(machine => machine.Line.Code).ThenBy(machine => machine.Code)
            .Select(machine => new SelectOptionViewModel { Id = machine.Id, Label = machine.Code + " - " + machine.Name })
            .ToArrayAsync(cancellationToken);

    public async Task<SelectOptionViewModel[]> ListAllMachinesAsync(CancellationToken cancellationToken) =>
        await _db.Machines
            .AsNoTracking()
            .OrderBy(machine => machine.Line.Facility.Code).ThenBy(machine => machine.Line.Code).ThenBy(machine => machine.Code)
            .Select(machine => new SelectOptionViewModel
            {
                Id = machine.Id,
                Label = machine.Line.Facility.Code + "-" + machine.Line.Code + "-" + machine.Code + " - " + machine.Name
            })
            .ToArrayAsync(cancellationToken);

    public async Task<SelectOptionViewModel[]> ListCleaningTypesAsync(CancellationToken cancellationToken) =>
        await _db.CleaningTypes
            .AsNoTracking()
            .OrderBy(type => type.Name)
            .Select(type => new SelectOptionViewModel { Id = type.Id, Label = type.Name })
            .ToArrayAsync(cancellationToken);

    public async Task<string?> FindInvalidSelectionAsync(int? facilityId, int? lineId, int? machineId, int? cleaningTypeId, CancellationToken cancellationToken)
    {
        if (facilityId is { } facility && !await _db.Facilities.AnyAsync(item => item.Id == facility, cancellationToken))
        {
            return ReportMessages.UnknownFacility;
        }

        if (lineId is { } line && !await _db.ProductionLines.AnyAsync(item => item.Id == line && (facilityId == null || item.FacilityId == facilityId), cancellationToken))
        {
            return ReportMessages.UnknownLine;
        }

        if (machineId is { } machine && !await _db.Machines.AnyAsync(item => item.Id == machine
            && (lineId == null || item.ProductionLineId == lineId) && (facilityId == null || item.Line.FacilityId == facilityId), cancellationToken))
        {
            return ReportMessages.UnknownMachine;
        }

        return cleaningTypeId is { } type && !await _db.CleaningTypes.AnyAsync(item => item.Id == type, cancellationToken)
            ? ReportMessages.UnknownCleaningType
            : null;
    }
}
