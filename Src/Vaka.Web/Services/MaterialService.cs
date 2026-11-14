using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;
using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Services;

public sealed class MaterialService : IMaterialService
{
    private const int PageSize = 20;
    private const string DuplicateCode = "Bu malzeme kodu kayıtlı.";

    private readonly VakaDbContext _db;
    private readonly IDurationCalculator _calculator;
    private readonly ITurkeyClock _clock;

    public MaterialService(VakaDbContext db, IDurationCalculator calculator, ITurkeyClock clock)
    {
        _db = db;
        _calculator = calculator;
        _clock = clock;
    }

    public async Task<MaterialListViewModel> ListAsync(string? query, bool showInactive, int page, CancellationToken cancellationToken)
    {
        var materials = _db.Materials.AsNoTracking().Where(material => showInactive || material.IsActive);
        var text = query?.Trim();
        if (!string.IsNullOrEmpty(text))
        {
            var pattern = $"%{text}%";
            materials = materials.Where(material => EF.Functions.Like(material.Code, pattern) || EF.Functions.Like(material.Name, pattern));
        }

        var total = await materials.CountAsync(cancellationToken);
        page = Math.Max(page, 1);
        var rows = await materials
            .OrderBy(material => material.Code)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(material => new
            {
                material.Id,
                material.Code,
                material.Name,
                material.IsActive,
                Expiries = material.Lots.Select(lot => lot.ExpiryDate).ToList()
            })
            .ToListAsync(cancellationToken);

        var today = _clock.Today();
        return new MaterialListViewModel
        {
            Items = rows.Select(row => new MaterialListItemViewModel
            {
                Id = row.Id,
                Code = row.Code,
                Name = row.Name,
                IsActive = row.IsActive,
                LotCount = row.Expiries.Count,
                ExpiredLotCount = row.Expiries.Count(expiry => _calculator.IsLotExpired(expiry, today))
            }).ToArray(),
            Query = text,
            ShowInactive = showInactive,
            Page = page,
            PageSize = PageSize,
            Total = total
        };
    }

    public async Task<LookupResult<MaterialFormViewModel>> GetFormAsync(int? id, CancellationToken cancellationToken)
    {
        if (id is null)
        {
            return LookupResult<MaterialFormViewModel>.Found(new MaterialFormViewModel());
        }

        var form = await _db.Materials
            .AsNoTracking()
            .Where(material => material.Id == id)
            .Select(material => new MaterialFormViewModel { Id = material.Id, Code = material.Code, Name = material.Name, IsActive = material.IsActive })
            .FirstOrDefaultAsync(cancellationToken);

        return form is null ? LookupResult<MaterialFormViewModel>.Missing() : LookupResult<MaterialFormViewModel>.Found(form);
    }

    public async Task<LookupResult<MaterialDetailViewModel>> GetDetailAsync(int id, CancellationToken cancellationToken)
    {
        var material = await _db.Materials
            .AsNoTracking()
            .Where(item => item.Id == id)
            .Select(item => new
            {
                item.Id,
                item.Code,
                item.Name,
                item.IsActive,
                Lots = item.Lots.OrderBy(lot => lot.ExpiryDate).ThenBy(lot => lot.LotNo).Select(lot => new { lot.Id, lot.LotNo, lot.ExpiryDate }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (material is null)
        {
            return LookupResult<MaterialDetailViewModel>.Missing();
        }

        var today = _clock.Today();
        return LookupResult<MaterialDetailViewModel>.Found(new MaterialDetailViewModel
        {
            Id = material.Id,
            Code = material.Code,
            Name = material.Name,
            IsActive = material.IsActive,
            Lots = material.Lots.Select(lot => new MaterialLotItemViewModel
            {
                Id = lot.Id,
                LotNo = lot.LotNo,
                ExpiryDate = lot.ExpiryDate,
                IsExpired = _calculator.IsLotExpired(lot.ExpiryDate, today)
            }).ToArray()
        });
    }

    public async Task<CommandResult> CreateAsync(MaterialRequest request, CancellationToken cancellationToken)
    {
        var errors = new List<FieldError>();
        var code = DefinitionRules.CheckCode(request.Code, "Malzeme kodu", DefinitionRules.MaterialCodeMaxLength, true, errors);
        var name = DefinitionRules.CheckName(request.Name, "Malzeme adı", DefinitionRules.NameMaxLength, errors);
        if (errors.Count == 0 && await _db.Materials.AnyAsync(material => material.Code == code, cancellationToken))
        {
            errors.Add(new FieldError("Code", DuplicateCode));
        }

        if (errors.Count > 0)
        {
            return CommandResult.Rejected(errors);
        }

        var material = new Material { Code = code, Name = name, IsActive = request.IsActive };
        _db.Materials.Add(material);
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return CommandResult.Rejected("Code", DuplicateCode);
        }

        return CommandResult.Success(material.Id);
    }

    public async Task<CommandResult> UpdateAsync(int id, MaterialRequest request, CancellationToken cancellationToken)
    {
        var material = await _db.Materials.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (material is null)
        {
            return CommandResult.NotFound();
        }

        var errors = new List<FieldError>();
        var code = DefinitionRules.CheckCode(request.Code, "Malzeme kodu", DefinitionRules.MaterialCodeMaxLength, true, errors);
        var name = DefinitionRules.CheckName(request.Name, "Malzeme adı", DefinitionRules.NameMaxLength, errors);
        if (errors.Count == 0
            && !string.Equals(code, material.Code, StringComparison.OrdinalIgnoreCase)
            && await _db.Materials.AnyAsync(other => other.Code == code && other.Id != id, cancellationToken))
        {
            errors.Add(new FieldError("Code", DuplicateCode));
        }

        if (errors.Count > 0)
        {
            return CommandResult.Rejected(errors);
        }

        material.Code = code;
        material.Name = name;
        material.IsActive = request.IsActive;
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return CommandResult.Rejected("Code", DuplicateCode);
        }

        return CommandResult.Success(id);
    }
}
