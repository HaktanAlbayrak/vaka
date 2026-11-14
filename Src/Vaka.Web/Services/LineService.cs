using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;
using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Services;

public sealed class LineService : ILineService
{
    private const string DuplicateCode = "Bu hat kodu tesiste kayıtlı.";
    private const string Label = "Hat kodu";

    private readonly VakaDbContext _db;

    public LineService(VakaDbContext db)
    {
        _db = db;
    }

    public async Task<LookupResult<LineFormViewModel>> GetFormAsync(int? id, int? facilityId, CancellationToken cancellationToken)
    {
        if (id is not null)
        {
            var form = await _db.ProductionLines
                .AsNoTracking()
                .Where(line => line.Id == id)
                .Select(line => new LineFormViewModel
                {
                    Id = line.Id,
                    FacilityId = line.FacilityId,
                    FacilityName = line.Facility.Name,
                    Code = line.Code,
                    Name = line.Name,
                    IsActive = line.IsActive,
                    CodeLocked = true
                })
                .FirstOrDefaultAsync(cancellationToken);

            return form is null ? LookupResult<LineFormViewModel>.Missing() : LookupResult<LineFormViewModel>.Found(form);
        }

        var facility = await _db.Facilities
            .AsNoTracking()
            .Where(item => item.Id == facilityId)
            .Select(item => new LineFormViewModel { FacilityId = item.Id, FacilityName = item.Name })
            .FirstOrDefaultAsync(cancellationToken);

        return facility is null ? LookupResult<LineFormViewModel>.Missing() : LookupResult<LineFormViewModel>.Found(facility);
    }

    public async Task<CommandResult> CreateAsync(int facilityId, LineRequest request, CancellationToken cancellationToken)
    {
        var facility = await _db.Facilities.AsNoTracking().FirstOrDefaultAsync(item => item.Id == facilityId, cancellationToken);
        if (facility is null)
        {
            return CommandResult.NotFound();
        }

        var errors = new List<FieldError>();
        if (!facility.IsActive)
        {
            errors.Add(new FieldError(string.Empty, "Pasif tesise hat eklenemez."));
        }

        var code = DefinitionRules.CheckCode(request.Code, Label, DefinitionRules.CodeMaxLength, false, errors);
        var name = DefinitionRules.CheckName(request.Name, "Hat adı", DefinitionRules.NameMaxLength, errors);
        if (errors.Count == 0 && await _db.ProductionLines.AnyAsync(line => line.FacilityId == facilityId && line.Code == code, cancellationToken))
        {
            errors.Add(new FieldError("Code", DuplicateCode));
        }

        if (errors.Count > 0)
        {
            return CommandResult.Rejected(errors);
        }

        _db.ProductionLines.Add(new ProductionLine { FacilityId = facilityId, Code = code, Name = name, IsActive = request.IsActive });
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return CommandResult.Rejected("Code", DuplicateCode);
        }

        return CommandResult.Success(facilityId);
    }

    public async Task<CommandResult> UpdateAsync(int id, LineRequest request, CancellationToken cancellationToken)
    {
        var line = await _db.ProductionLines.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (line is null)
        {
            return CommandResult.NotFound();
        }

        var errors = new List<FieldError>();
        if (DefinitionRules.IsCodeChange(request.Code, line.Code))
        {
            errors.Add(new FieldError("Code", "Hat kodu kayıttan sonra değiştirilemez."));
        }

        var name = DefinitionRules.CheckName(request.Name, "Hat adı", DefinitionRules.NameMaxLength, errors);
        if (errors.Count > 0)
        {
            return CommandResult.Rejected(errors);
        }

        line.Name = name;
        line.IsActive = request.IsActive;
        await _db.SaveChangesAsync(cancellationToken);
        return CommandResult.Success(line.FacilityId);
    }
}
