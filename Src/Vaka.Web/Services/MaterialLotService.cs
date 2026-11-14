using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;
using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Services;

public sealed class MaterialLotService : IMaterialLotService
{
    private const string DuplicateLot = "Bu lot no malzemede kayıtlı.";

    private readonly VakaDbContext _db;

    public MaterialLotService(VakaDbContext db)
    {
        _db = db;
    }

    public async Task<LookupResult<MaterialLotFormViewModel>> GetFormAsync(int? id, int? materialId, CancellationToken cancellationToken)
    {
        MaterialLotFormViewModel? form;
        if (id is not null)
        {
            form = await _db.MaterialLots
                .AsNoTracking()
                .Where(lot => lot.Id == id)
                .Select(lot => new MaterialLotFormViewModel
                {
                    Id = lot.Id,
                    MaterialId = lot.MaterialId,
                    MaterialName = lot.Material.Name,
                    LotNo = lot.LotNo,
                    ExpiryDate = lot.ExpiryDate
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
        else
        {
            form = await _db.Materials
                .AsNoTracking()
                .Where(material => material.Id == materialId)
                .Select(material => new MaterialLotFormViewModel { MaterialId = material.Id, MaterialName = material.Name })
                .FirstOrDefaultAsync(cancellationToken);
        }

        return form is null ? LookupResult<MaterialLotFormViewModel>.Missing() : LookupResult<MaterialLotFormViewModel>.Found(form);
    }

    public async Task<CommandResult> CreateAsync(int materialId, MaterialLotRequest request, CancellationToken cancellationToken)
    {
        if (!await _db.Materials.AnyAsync(material => material.Id == materialId, cancellationToken))
        {
            return CommandResult.NotFound();
        }

        var errors = new List<FieldError>();
        var lotNo = Check(request, errors);
        if (errors.Count == 0 && await _db.MaterialLots.AnyAsync(lot => lot.MaterialId == materialId && lot.LotNo == lotNo, cancellationToken))
        {
            errors.Add(new FieldError("LotNo", DuplicateLot));
        }

        if (errors.Count > 0)
        {
            return CommandResult.Rejected(errors);
        }

        _db.MaterialLots.Add(new MaterialLot { MaterialId = materialId, LotNo = lotNo, ExpiryDate = request.ExpiryDate!.Value });
        return await SaveAsync(materialId, cancellationToken);
    }

    public async Task<CommandResult> UpdateAsync(int id, MaterialLotRequest request, CancellationToken cancellationToken)
    {
        var lot = await _db.MaterialLots.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (lot is null)
        {
            return CommandResult.NotFound();
        }

        var errors = new List<FieldError>();
        var lotNo = Check(request, errors);
        if (errors.Count == 0 && await _db.MaterialLots.AnyAsync(other => other.MaterialId == lot.MaterialId && other.LotNo == lotNo && other.Id != id, cancellationToken))
        {
            errors.Add(new FieldError("LotNo", DuplicateLot));
        }

        if (errors.Count > 0)
        {
            return CommandResult.Rejected(errors);
        }

        lot.LotNo = lotNo;
        lot.ExpiryDate = request.ExpiryDate!.Value;
        return await SaveAsync(lot.MaterialId, cancellationToken);
    }

    private async Task<CommandResult> SaveAsync(int materialId, CancellationToken cancellationToken)
    {
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return CommandResult.Rejected("LotNo", DuplicateLot);
        }

        return CommandResult.Success(materialId);
    }

    private static string Check(MaterialLotRequest request, List<FieldError> errors)
    {
        var lotNo = DefinitionRules.CheckCode(request.LotNo, "Lot no", DefinitionRules.LotNoMaxLength, true, errors, "LotNo");
        if (request.ExpiryDate is null)
        {
            errors.Add(new FieldError("ExpiryDate", "SKT girin."));
        }

        return lotNo;
    }
}
