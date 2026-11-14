using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaka.Web.Authorization;
using Vaka.Web.Services;
using Vaka.Web.ViewModels.Users;

namespace Vaka.Web.Controllers;

[Authorize(Roles = Roles.ManagerOrSuperAdmin)]
[Route("[controller]/[action]")]
public sealed class UsersController : Controller
{
    private const string StatusKey = "StatusMessage";

    private readonly IUserDirectory _directory;
    private readonly IUserAdministration _administration;

    public UsersController(IUserDirectory directory, IUserAdministration administration)
    {
        _directory = directory;
        _administration = administration;
    }

    [HttpGet("~/users")]
    public async Task<IActionResult> Index(
        string? query,
        UserStatusFilter status = UserStatusFilter.Active,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var model = await _directory.ListAsync(User.GetUserId(), query, status, page, cancellationToken);
        model.StatusMessage = TempData[StatusKey] as string;
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken) =>
        await ShowFormAsync(null, null, cancellationToken);

    [HttpPost]
    public async Task<IActionResult> Create(UserFormViewModel model, CancellationToken cancellationToken)
    {
        var request = new UserCreateRequest(model.RegistryNo, model.FullName, model.SelectedRoles, model.TemporaryPassword);
        var result = await _administration.CreateAsync(User.GetUserId(), request, cancellationToken);
        if (result.Succeeded)
        {
            TempData[StatusKey] = "Kullanıcı eklendi.";
            return RedirectToAction(nameof(Index));
        }

        return await RejectAsync(result, null, model, string.Empty, cancellationToken);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken) =>
        await ShowFormAsync(id, null, cancellationToken);

    [HttpPost("{id:int}")]
    public async Task<IActionResult> Edit(int id, UserFormViewModel model, CancellationToken cancellationToken)
    {
        var request = new UserUpdateRequest(model.FullName, model.SelectedRoles);
        var result = await _administration.UpdateAsync(User.GetUserId(), id, request, cancellationToken);
        if (result.Succeeded)
        {
            TempData[StatusKey] = "Kullanıcı güncellendi.";
            return RedirectToAction(nameof(Edit), new { id });
        }

        return await RejectAsync(result, id, model, string.Empty, cancellationToken);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
    {
        var lookup = await _directory.GetDeactivateAsync(User.GetUserId(), id, cancellationToken);
        return lookup.Status == CommandStatus.Succeeded ? View(lookup.Value) : LookupFailure(lookup.Status);
    }

    [HttpPost("{id:int}")]
    public async Task<IActionResult> Deactivate(int id, string? pin, CancellationToken cancellationToken)
    {
        var result = await _administration.DeactivateAsync(User.GetUserId(), id, pin, cancellationToken);
        if (result.Succeeded)
        {
            TempData[StatusKey] = "Kullanıcı pasifleştirildi.";
            return RedirectToAction(nameof(Index));
        }

        if (await TerminalFailureAsync(result) is { } terminal)
        {
            return terminal;
        }

        ModelState.AddErrors(result);
        var lookup = await _directory.GetDeactivateAsync(User.GetUserId(), id, cancellationToken);
        return lookup.Status == CommandStatus.Succeeded ? View(lookup.Value) : LookupFailure(lookup.Status);
    }

    [HttpPost("{id:int}")]
    public async Task<IActionResult> Activate(int id, [Bind(Prefix = "Activate")] UserResetViewModel model, CancellationToken cancellationToken)
    {
        var result = await _administration.ActivateAsync(User.GetUserId(), id, model.Pin, cancellationToken);
        return await FinishResetAsync(result, id, "Activate", "Kullanıcı yeniden etkinleştirildi.", cancellationToken);
    }

    [HttpPost("{id:int}")]
    public async Task<IActionResult> ResetPin(int id, [Bind(Prefix = "ResetPin")] UserResetViewModel model, CancellationToken cancellationToken)
    {
        var result = await _administration.ResetPinAsync(User.GetUserId(), id, model.Pin, cancellationToken);
        return await FinishResetAsync(result, id, "ResetPin", "PIN sıfırlandı. Kullanıcı sonraki girişte yeni PIN belirler.", cancellationToken);
    }

    [HttpPost("{id:int}")]
    public async Task<IActionResult> ResetPassword(int id, [Bind(Prefix = "ResetPassword")] UserResetViewModel model, CancellationToken cancellationToken)
    {
        var result = await _administration.ResetPasswordAsync(User.GetUserId(), id, model.TemporaryPassword, model.Pin, cancellationToken);
        return await FinishResetAsync(result, id, "ResetPassword", "Geçici parola yazıldı. Kullanıcı sonraki girişte parolasını değiştirir.", cancellationToken);
    }

    private async Task<IActionResult> FinishResetAsync(
        CommandResult result,
        int id,
        string prefix,
        string successMessage,
        CancellationToken cancellationToken)
    {
        if (result.Succeeded)
        {
            TempData[StatusKey] = successMessage;
            return RedirectToAction(nameof(Edit), new { id });
        }

        return await RejectAsync(result, id, null, prefix, cancellationToken);
    }

    private async Task<IActionResult> RejectAsync(
        CommandResult result,
        int? targetId,
        UserFormViewModel? posted,
        string prefix,
        CancellationToken cancellationToken)
    {
        if (await TerminalFailureAsync(result) is { } terminal)
        {
            return terminal;
        }

        ModelState.AddErrors(result, prefix);
        return await ShowFormAsync(targetId, posted, cancellationToken);
    }

    private async Task<IActionResult> ShowFormAsync(int? targetId, UserFormViewModel? posted, CancellationToken cancellationToken)
    {
        var lookup = await _directory.GetFormAsync(User.GetUserId(), targetId, cancellationToken);
        if (lookup.Status != CommandStatus.Succeeded)
        {
            return LookupFailure(lookup.Status);
        }

        var model = lookup.Value!;
        if (posted is not null)
        {
            model.FullName = posted.FullName;
            if (targetId is null)
            {
                model.RegistryNo = posted.RegistryNo;
            }

            if (!model.RolesLocked)
            {
                model.SelectedRoles = posted.SelectedRoles;
            }
        }
        else
        {
            model.StatusMessage = TempData[StatusKey] as string;
        }

        return View("Form", model);
    }

    private async Task<IActionResult?> TerminalFailureAsync(CommandResult result) => result.Status switch
    {
        CommandStatus.Forbidden => Forbid(),
        CommandStatus.NotFound => NotFound(),
        CommandStatus.PinLocked => await this.SignOutToLoginAsync(SessionEndReason.PinLocked),
        _ => null
    };

    private IActionResult LookupFailure(CommandStatus status) =>
        status == CommandStatus.Forbidden ? Forbid() : NotFound();
}
