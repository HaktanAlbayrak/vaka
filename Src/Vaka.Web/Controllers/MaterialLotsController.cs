using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaka.Web.Authorization;
using Vaka.Web.Services;
using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Controllers;

[Authorize(Roles = Roles.Manager)]
[Route("[controller]/[action]")]
public sealed class MaterialLotsController : Controller
{
    private const string StatusKey = "StatusMessage";

    private readonly IMaterialLotService _lots;

    public MaterialLotsController(IMaterialLotService lots)
    {
        _lots = lots;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? materialId, CancellationToken cancellationToken) =>
        await ShowFormAsync(null, materialId, null, cancellationToken);

    [HttpPost]
    public async Task<IActionResult> Create(MaterialLotFormViewModel model, CancellationToken cancellationToken)
    {
        var result = await _lots.CreateAsync(model.MaterialId, ToRequest(model), cancellationToken);
        if (result.Succeeded)
        {
            TempData[StatusKey] = "Lot eklendi.";
            return RedirectToAction("Details", "Materials", new { id = result.EntityId });
        }

        return await RejectAsync(result, null, model, cancellationToken);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken) =>
        await ShowFormAsync(id, null, null, cancellationToken);

    [HttpPost("{id:int}")]
    public async Task<IActionResult> Edit(int id, MaterialLotFormViewModel model, CancellationToken cancellationToken)
    {
        var result = await _lots.UpdateAsync(id, ToRequest(model), cancellationToken);
        if (result.Succeeded)
        {
            TempData[StatusKey] = "Lot güncellendi.";
            return RedirectToAction("Details", "Materials", new { id = result.EntityId });
        }

        return await RejectAsync(result, id, model, cancellationToken);
    }

    // Çözülemeyen tarih modelde boş gelir; hata iletisini servis Türkçe verir, bağlayıcının İngilizce iletisi atılır.
    private MaterialLotRequest ToRequest(MaterialLotFormViewModel model)
    {
        ModelState.Remove(nameof(MaterialLotFormViewModel.ExpiryDate));
        return new MaterialLotRequest(model.LotNo, model.ExpiryDate);
    }

    private async Task<IActionResult> RejectAsync(
        CommandResult result,
        int? id,
        MaterialLotFormViewModel posted,
        CancellationToken cancellationToken)
    {
        if (result.Status == CommandStatus.NotFound)
        {
            return NotFound();
        }

        ModelState.AddErrors(result);
        return await ShowFormAsync(id, posted.MaterialId, posted, cancellationToken);
    }

    private async Task<IActionResult> ShowFormAsync(int? id, int? materialId, MaterialLotFormViewModel? posted, CancellationToken cancellationToken)
    {
        var lookup = await _lots.GetFormAsync(id, materialId, cancellationToken);
        if (lookup.Status != CommandStatus.Succeeded)
        {
            return NotFound();
        }

        var model = lookup.Value!;
        if (posted is not null)
        {
            model.LotNo = posted.LotNo;
            model.ExpiryDate = posted.ExpiryDate;
        }

        return View("Form", model);
    }
}
