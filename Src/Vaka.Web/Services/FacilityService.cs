using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;
using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Services;

public sealed class FacilityService : IFacilityService
{
    private const string DuplicateCode = "Bu tesis kodu kayıtlı.";
    private const string Label = "Tesis kodu";

    private readonly VakaDbContext _db;

    public FacilityService(VakaDbContext db)
    {
        _db = db;
    }

    public async Task<FacilityListViewModel> ListAsync(bool showInactive, CancellationToken cancellationToken)
    {
        var items = await _db.Facilities
            .AsNoTracking()
            .Where(facility => showInactive || facility.IsActive)
            .OrderBy(facility => facility.Code)
            .Select(facility => new FacilityListItemViewModel
            {
                Id = facility.Id,
                Code = facility.Code,
                Name = facility.Name,
                IsActive = facility.IsActive,
                LineCount = facility.Lines.Count,
                ShiftCount = facility.Shifts.Count
            })
            .ToArrayAsync(cancellationToken);

        return new FacilityListViewModel { Items = items, ShowInactive = showInactive };
    }

    public async Task<LookupResult<FacilityFormViewModel>> GetFormAsync(int? id, CancellationToken cancellationToken)
    {
        if (id is null)
        {
            return LookupResult<FacilityFormViewModel>.Found(new FacilityFormViewModel());
        }

        var form = await _db.Facilities
            .AsNoTracking()
            .Where(facility => facility.Id == id)
            .Select(facility => new FacilityFormViewModel
            {
                Id = facility.Id,
                Code = facility.Code,
                Name = facility.Name,
                IsActive = facility.IsActive,
                CodeLocked = true
            })
            .FirstOrDefaultAsync(cancellationToken);

        return form is null ? LookupResult<FacilityFormViewModel>.Missing() : LookupResult<FacilityFormViewModel>.Found(form);
    }

    public async Task<LookupResult<FacilityDetailViewModel>> GetDetailAsync(int id, CancellationToken cancellationToken)
    {
        var detail = await _db.Facilities
            .AsNoTracking()
            .Where(facility => facility.Id == id)
            .Select(facility => new FacilityDetailViewModel
            {
                Id = facility.Id,
                Code = facility.Code,
                Name = facility.Name,
                IsActive = facility.IsActive,
                Lines = facility.Lines
                    .OrderBy(line => line.Code)
                    .Select(line => new FacilityLineItemViewModel
                    {
                        Id = line.Id,
                        Code = line.Code,
                        Name = line.Name,
                        IsActive = line.IsActive,
                        MachineCount = line.Machines.Count
                    })
                    .ToArray()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return detail is null ? LookupResult<FacilityDetailViewModel>.Missing() : LookupResult<FacilityDetailViewModel>.Found(detail);
    }

    public async Task<CommandResult> CreateAsync(FacilityRequest request, CancellationToken cancellationToken)
    {
        var errors = new List<FieldError>();
        var code = DefinitionRules.CheckCode(request.Code, Label, DefinitionRules.CodeMaxLength, false, errors);
        var name = DefinitionRules.CheckName(request.Name, "Tesis adı", DefinitionRules.NameMaxLength, errors);
        if (errors.Count == 0 && await _db.Facilities.AnyAsync(facility => facility.Code == code, cancellationToken))
        {
            errors.Add(new FieldError("Code", DuplicateCode));
        }

        if (errors.Count > 0)
        {
            return CommandResult.Rejected(errors);
        }

        var facility = new Facility { Code = code, Name = name, IsActive = request.IsActive };
        _db.Facilities.Add(facility);
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return CommandResult.Rejected("Code", DuplicateCode);
        }

        return CommandResult.Success(facility.Id);
    }

    public async Task<CommandResult> UpdateAsync(int id, FacilityRequest request, CancellationToken cancellationToken)
    {
        var facility = await _db.Facilities.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (facility is null)
        {
            return CommandResult.NotFound();
        }

        var errors = new List<FieldError>();
        if (DefinitionRules.IsCodeChange(request.Code, facility.Code))
        {
            errors.Add(new FieldError("Code", "Tesis kodu kayıttan sonra değiştirilemez."));
        }

        var name = DefinitionRules.CheckName(request.Name, "Tesis adı", DefinitionRules.NameMaxLength, errors);
        if (errors.Count > 0)
        {
            return CommandResult.Rejected(errors);
        }

        facility.Name = name;
        facility.IsActive = request.IsActive;
        await _db.SaveChangesAsync(cancellationToken);
        return CommandResult.Success(id);
    }
}
