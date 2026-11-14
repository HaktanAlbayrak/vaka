using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;
using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Services;

public sealed class MachineService : IMachineService
{
    private const int PageSize = 20;
    private const string DuplicateCode = "Bu makine kodu hatta kayıtlı.";

    private readonly VakaDbContext _db;
    private readonly ITurkeyClock _clock;

    public MachineService(VakaDbContext db, ITurkeyClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<MachineListViewModel> ListAsync(MachineListQuery query, CancellationToken cancellationToken)
    {
        var machines = _db.Machines.AsNoTracking().AsQueryable();
        if (!query.ShowRetired)
        {
            machines = machines.Where(machine => machine.RetiredAtUtc == null);
        }

        var lineId = query.LineId;
        var facilityId = query.FacilityId;
        if (lineId is not null)
        {
            var lineFacility = await _db.ProductionLines
                .AsNoTracking()
                .Where(line => line.Id == lineId && (facilityId == null || line.FacilityId == facilityId))
                .Select(line => (int?)line.FacilityId)
                .FirstOrDefaultAsync(cancellationToken);

            if (lineFacility is null)
            {
                lineId = null;
            }
            else
            {
                facilityId = lineFacility;
                machines = machines.Where(machine => machine.ProductionLineId == lineId);
            }
        }

        if (lineId is null && facilityId is not null)
        {
            machines = machines.Where(machine => machine.Line.FacilityId == facilityId);
        }

        var text = query.Query?.Trim();
        if (!string.IsNullOrEmpty(text))
        {
            var pattern = $"%{text}%";
            machines = machines.Where(machine => EF.Functions.Like(machine.Code, pattern) || EF.Functions.Like(machine.Name, pattern));
        }

        var total = await machines.CountAsync(cancellationToken);
        var page = Math.Max(query.Page, 1);
        var rows = await machines
            .OrderBy(machine => machine.Line.Facility.Code)
            .ThenBy(machine => machine.Line.Code)
            .ThenBy(machine => machine.Code)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(machine => new
            {
                machine.Id,
                machine.Code,
                machine.Name,
                FacilityName = machine.Line.Facility.Name,
                LineName = machine.Line.Name,
                machine.IsActive,
                machine.RetiredAtUtc
            })
            .ToListAsync(cancellationToken);

        return new MachineListViewModel
        {
            Items = rows.Select(row => new MachineListItemViewModel
            {
                Id = row.Id,
                Code = row.Code,
                Name = row.Name,
                FacilityName = row.FacilityName,
                LineName = row.LineName,
                IsActive = row.IsActive,
                RetiredAtLocal = row.RetiredAtUtc is { } retired ? _clock.ToLocal(retired) : null
            }).ToArray(),
            FacilityId = facilityId,
            LineId = lineId,
            Query = text,
            ShowRetired = query.ShowRetired,
            Facilities = await ListFacilitiesAsync(false, cancellationToken),
            Lines = await ListLinesCoreAsync(facilityId, false, cancellationToken),
            Page = page,
            PageSize = PageSize,
            Total = total
        };
    }

    public async Task<LookupResult<MachineFormViewModel>> GetFormAsync(int? id, int? facilityId, CancellationToken cancellationToken)
    {
        if (id is not null)
        {
            var stored = await _db.Machines
                .AsNoTracking()
                .Where(machine => machine.Id == id)
                .Select(machine => new
                {
                    machine.Id,
                    machine.Code,
                    machine.Name,
                    machine.RetiredAtUtc,
                    machine.ProductionLineId,
                    FacilityId = machine.Line.FacilityId,
                    FacilityLabel = machine.Line.Facility.Code + " - " + machine.Line.Facility.Name,
                    LineLabel = machine.Line.Code + " - " + machine.Line.Name
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (stored is null)
            {
                return LookupResult<MachineFormViewModel>.Missing();
            }

            return LookupResult<MachineFormViewModel>.Found(new MachineFormViewModel
            {
                Id = stored.Id,
                FacilityId = stored.FacilityId,
                LineId = stored.ProductionLineId,
                Code = stored.Code,
                Name = stored.Name,
                CodeLocked = true,
                RetiredAtLocal = stored.RetiredAtUtc is { } retired ? _clock.ToLocal(retired) : null,
                Facilities = [new SelectOptionViewModel { Id = stored.FacilityId, Label = stored.FacilityLabel }],
                Lines = [new SelectOptionViewModel { Id = stored.ProductionLineId, Label = stored.LineLabel }]
            });
        }

        var facilities = await ListFacilitiesAsync(true, cancellationToken);
        var selected = facilities.Any(facility => facility.Id == facilityId) ? facilityId!.Value : 0;
        return LookupResult<MachineFormViewModel>.Found(new MachineFormViewModel
        {
            FacilityId = selected,
            Facilities = facilities,
            Lines = selected == 0 ? [] : await ListLinesCoreAsync(selected, true, cancellationToken)
        });
    }

    public async Task<SelectOptionViewModel[]> ListLinesAsync(int facilityId, CancellationToken cancellationToken) =>
        await ListLinesCoreAsync(facilityId, true, cancellationToken);

    public async Task<CommandResult> CreateAsync(MachineRequest request, CancellationToken cancellationToken)
    {
        var errors = new List<FieldError>();
        var line = await _db.ProductionLines
            .AsNoTracking()
            .Include(item => item.Facility)
            .FirstOrDefaultAsync(item => item.Id == request.LineId, cancellationToken);

        if (request.FacilityId == 0 || !await _db.Facilities.AnyAsync(facility => facility.Id == request.FacilityId && facility.IsActive, cancellationToken))
        {
            errors.Add(new FieldError("FacilityId", "Aktif bir tesis seçin."));
        }
        else if (line is null)
        {
            errors.Add(new FieldError("LineId", "Hat seçin."));
        }
        else if (line.FacilityId != request.FacilityId)
        {
            errors.Add(new FieldError("LineId", "Seçilen hat bu tesise ait değil."));
        }
        else if (!line.IsActive)
        {
            errors.Add(new FieldError("LineId", "Pasif hatta makine eklenemez."));
        }

        var code = DefinitionRules.CheckCode(request.Code, "Makine kodu", DefinitionRules.CodeMaxLength, false, errors);
        var name = DefinitionRules.CheckName(request.Name, "Makine adı", DefinitionRules.NameMaxLength, errors);
        if (errors.Count == 0 && await _db.Machines.AnyAsync(machine => machine.ProductionLineId == request.LineId && machine.Code == code, cancellationToken))
        {
            errors.Add(new FieldError("Code", DuplicateCode));
        }

        if (errors.Count > 0)
        {
            return CommandResult.Rejected(errors);
        }

        var machine = new Machine { ProductionLineId = request.LineId, Code = code, Name = name, IsActive = true };
        _db.Machines.Add(machine);
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return CommandResult.Rejected("Code", DuplicateCode);
        }

        return CommandResult.Success(machine.Id);
    }

    public async Task<CommandResult> UpdateAsync(int id, MachineRequest request, CancellationToken cancellationToken)
    {
        var machine = await _db.Machines
            .Include(item => item.Line)
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (machine is null)
        {
            return CommandResult.NotFound();
        }

        if (machine.RetiredAtUtc is not null)
        {
            return CommandResult.Rejected(string.Empty, "Kullanımdan kaldırılmış makine değiştirilemez.");
        }

        var errors = new List<FieldError>();
        if (DefinitionRules.IsCodeChange(request.Code, machine.Code))
        {
            errors.Add(new FieldError("Code", "Makine kodu kayıttan sonra değiştirilemez."));
        }

        if ((request.LineId != 0 && request.LineId != machine.ProductionLineId)
            || (request.FacilityId != 0 && request.FacilityId != machine.Line.FacilityId))
        {
            errors.Add(new FieldError("LineId", "Makinenin hattı kayıttan sonra değiştirilemez."));
        }

        var name = DefinitionRules.CheckName(request.Name, "Makine adı", DefinitionRules.NameMaxLength, errors);
        if (errors.Count > 0)
        {
            return CommandResult.Rejected(errors);
        }

        machine.Name = name;
        await _db.SaveChangesAsync(cancellationToken);
        return CommandResult.Success(id);
    }

    private async Task<SelectOptionViewModel[]> ListFacilitiesAsync(bool activeOnly, CancellationToken cancellationToken) =>
        await _db.Facilities
            .AsNoTracking()
            .Where(facility => !activeOnly || facility.IsActive)
            .OrderBy(facility => facility.Code)
            .Select(facility => new SelectOptionViewModel { Id = facility.Id, Label = facility.Code + " - " + facility.Name })
            .ToArrayAsync(cancellationToken);

    private async Task<SelectOptionViewModel[]> ListLinesCoreAsync(int? facilityId, bool activeOnly, CancellationToken cancellationToken) =>
        await _db.ProductionLines
            .AsNoTracking()
            .Where(line => (facilityId == null || line.FacilityId == facilityId) && (!activeOnly || (line.IsActive && line.Facility.IsActive)))
            .OrderBy(line => line.Facility.Code)
            .ThenBy(line => line.Code)
            .Select(line => new SelectOptionViewModel
            {
                Id = line.Id,
                Label = facilityId == null ? line.Facility.Code + "-" + line.Code + " - " + line.Name : line.Code + " - " + line.Name
            })
            .ToArrayAsync(cancellationToken);
}
