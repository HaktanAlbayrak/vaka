using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Vaka.Web.Authorization;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

public sealed class UserAdministration : IUserAdministration
{
    private const int RegistryNoMaxLength = 32;
    private const int FullNameMaxLength = 120;
    private const int SqliteBusy = 5;
    private const int SqliteLocked = 6;
    private const string DuplicateRegistryNo = "Bu sicil no kayıtlı.";

    private readonly VakaDbContext _db;
    private readonly ICredentialHasher _hasher;
    private readonly IPinService _pins;
    private readonly IOpenWorkProvider _openWork;
    private readonly TimeProvider _time;

    public UserAdministration(
        VakaDbContext db,
        ICredentialHasher hasher,
        IPinService pins,
        IOpenWorkProvider openWork,
        TimeProvider time)
    {
        _db = db;
        _hasher = hasher;
        _pins = pins;
        _openWork = openWork;
        _time = time;
    }

    public async Task<CommandResult> CreateAsync(int actorId, UserCreateRequest request, CancellationToken cancellationToken)
    {
        var actorRoles = await UserQueries.GetRolesAsync(_db, actorId, cancellationToken);
        var assignable = ManagementScope.AssignableRoles(actorRoles);
        if (assignable.Length == 0)
        {
            return CommandResult.Forbidden();
        }

        var errors = new List<FieldError>();
        var registryNo = request.RegistryNo?.Trim() ?? string.Empty;
        if (registryNo.Length == 0)
        {
            errors.Add(new FieldError("RegistryNo", "Sicil no girin."));
        }
        else if (registryNo.Length > RegistryNoMaxLength)
        {
            errors.Add(new FieldError("RegistryNo", $"Sicil no en çok {RegistryNoMaxLength} karakter olabilir."));
        }
        else if (await _db.Users.AnyAsync(user => user.RegistryNo == registryNo, cancellationToken))
        {
            errors.Add(new FieldError("RegistryNo", DuplicateRegistryNo));
        }

        var fullName = CheckFullName(request.FullName, errors);
        var roles = CheckRoles(request.Roles, assignable, errors);

        var passwordError = PasswordRules.Validate(request.TemporaryPassword);
        if (passwordError is not null)
        {
            errors.Add(new FieldError("TemporaryPassword", passwordError));
        }

        if (errors.Count > 0)
        {
            return CommandResult.Rejected(errors);
        }

        var user = new User
        {
            RegistryNo = registryNo,
            FullName = fullName,
            PasswordHash = _hasher.Hash(request.TemporaryPassword!),
            MustChangePassword = true,
            MustSetPin = true,
            IsActive = true,
            CreatedAtUtc = _time.GetUtcNow().UtcDateTime,
            Roles = roles.Select(role => new UserRole { Role = role }).ToList()
        };
        _db.Users.Add(user);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return CommandResult.Rejected("RegistryNo", DuplicateRegistryNo);
        }

        return CommandResult.Success(user.Id);
    }

    public async Task<CommandResult> UpdateAsync(int actorId, int targetId, UserUpdateRequest request, CancellationToken cancellationToken)
    {
        return await RunExclusiveAsync(actorId, targetId, async (target, actorRoles) =>
        {
            var errors = new List<FieldError>();
            var fullName = CheckFullName(request.FullName, errors);

            string[]? newRoles = null;
            if (actorId != targetId)
            {
                newRoles = CheckRoles(request.Roles, ManagementScope.AssignableRoles(actorRoles), errors);
                if (errors.Count == 0
                    && target.Roles.Any(role => role.Role == Roles.SuperAdmin)
                    && !newRoles.Contains(Roles.SuperAdmin)
                    && await UserQueries.IsLastActiveSuperAdminAsync(_db, targetId, cancellationToken))
                {
                    errors.Add(new FieldError("SelectedRoles", "Son aktif Süper admin'in Süper admin rolü kaldırılamaz."));
                }
            }

            if (errors.Count > 0)
            {
                return CommandResult.Rejected(errors);
            }

            target.FullName = fullName;
            if (newRoles is not null)
            {
                _db.UserRoles.RemoveRange(target.Roles.Where(role => !newRoles.Contains(role.Role)));
                foreach (var role in newRoles.Where(role => target.Roles.All(existing => existing.Role != role)))
                {
                    _db.UserRoles.Add(new UserRole { UserId = targetId, Role = role });
                }
            }

            await _db.SaveChangesAsync(cancellationToken);
            return CommandResult.Success(targetId);
        }, cancellationToken);
    }

    public async Task<CommandResult> DeactivateAsync(int actorId, int targetId, string? pin, CancellationToken cancellationToken)
    {
        var resolved = await ResolveTargetAsync(actorId, targetId, cancellationToken);
        if (resolved.Failure is not null)
        {
            return resolved.Failure;
        }

        var block = await GetDeactivateBlockAsync(actorId, targetId, resolved.Target!, cancellationToken);
        if (block is not null)
        {
            return CommandResult.Rejected(string.Empty, block);
        }

        var pinFailure = await CheckPinAsync(actorId, pin, cancellationToken);
        if (pinFailure is not null)
        {
            return pinFailure;
        }

        return await RunExclusiveAsync(actorId, targetId, async (target, _) =>
        {
            var lockedBlock = await GetDeactivateBlockAsync(actorId, targetId, target, cancellationToken);
            if (lockedBlock is not null)
            {
                return CommandResult.Rejected(string.Empty, lockedBlock);
            }

            target.IsActive = false;
            await _db.SaveChangesAsync(cancellationToken);
            return CommandResult.Success(targetId);
        }, cancellationToken);
    }

    private async Task<string?> GetDeactivateBlockAsync(int actorId, int targetId, User target, CancellationToken cancellationToken)
    {
        var block = await UserQueries.GetDeactivateBlockAsync(_db, actorId, targetId, target.IsActive, cancellationToken);
        if (block is null && (await _openWork.ListAsync(targetId, actorId, cancellationToken)).Length > 0)
        {
            block = "Kullanıcının açık işi var. Önce bu işler kapatılmalı.";
        }

        return block;
    }

    /// <summary>
    /// PL-29: aktif Süper admin sayısını azaltabilen işlemler (pasifleştirme, rol güncelleme) başta yazma kilidi alan tek işlemde çalışır.
    /// Microsoft.Data.Sqlite hem Unspecified (EF varsayılanı) hem Serializable yalıtımda yazma kilidini hemen alır (<c>BEGIN IMMEDIATE</c>, denendi);
    /// Serializable niyeti açık göstermek için yazıldı.
    /// Kilitten sonra izleyici temizlenir; işlemi yapan kişi, hedef ve roller veritabanından yeniden okunur, işlemi yapan artık aktif değilse
    /// ya da kapsamı kalmadıysa değişiklik yapılmaz. Kilidi bekleyen istek zaman aşımında anlamlı iş hatası alır.
    /// </summary>
    private async Task<CommandResult> RunExclusiveAsync(
        int actorId,
        int targetId,
        Func<User, string[], Task<CommandResult>> action,
        CancellationToken cancellationToken)
    {
        try
        {
            await using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            _db.ChangeTracker.Clear();

            if (!await UserQueries.IsActiveAsync(_db, actorId, cancellationToken))
            {
                return CommandResult.Forbidden();
            }

            var resolved = await ResolveTargetAsync(actorId, targetId, cancellationToken);
            if (resolved.Failure is not null)
            {
                return resolved.Failure;
            }

            var result = await action(resolved.Target!, resolved.ActorRoles);
            if (result.Succeeded)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            return result;
        }
        catch (SqliteException exception) when (exception.SqliteErrorCode is SqliteBusy or SqliteLocked)
        {
            return CommandResult.Rejected(string.Empty, "Sistem şu an meşgul. İşlemi birkaç saniye sonra yeniden deneyin.");
        }
    }

    public async Task<CommandResult> ActivateAsync(int actorId, int targetId, string? pin, CancellationToken cancellationToken)
    {
        var resolved = await ResolveTargetAsync(actorId, targetId, cancellationToken);
        if (resolved.Failure is not null)
        {
            return resolved.Failure;
        }

        var target = resolved.Target!;
        if (target.IsActive)
        {
            return CommandResult.Rejected(string.Empty, "Hesap zaten etkin.");
        }

        var pinFailure = await CheckPinAsync(actorId, pin, cancellationToken);
        if (pinFailure is not null)
        {
            return pinFailure;
        }

        target.IsActive = true;
        await _db.SaveChangesAsync(cancellationToken);
        return CommandResult.Success(targetId);
    }

    public async Task<CommandResult> ResetPinAsync(int actorId, int targetId, string? pin, CancellationToken cancellationToken)
    {
        var resolved = await ResolveTargetAsync(actorId, targetId, cancellationToken);
        if (resolved.Failure is not null)
        {
            return resolved.Failure;
        }

        var pinFailure = await CheckPinAsync(actorId, pin, cancellationToken);
        if (pinFailure is not null)
        {
            return pinFailure;
        }

        await _pins.ResetAsync(targetId, cancellationToken);
        return CommandResult.Success(targetId);
    }

    public async Task<CommandResult> ResetPasswordAsync(
        int actorId,
        int targetId,
        string? temporaryPassword,
        string? pin,
        CancellationToken cancellationToken)
    {
        var resolved = await ResolveTargetAsync(actorId, targetId, cancellationToken);
        if (resolved.Failure is not null)
        {
            return resolved.Failure;
        }

        var passwordError = PasswordRules.Validate(temporaryPassword);
        if (passwordError is not null)
        {
            return CommandResult.Rejected("TemporaryPassword", passwordError);
        }

        var pinFailure = await CheckPinAsync(actorId, pin, cancellationToken);
        if (pinFailure is not null)
        {
            return pinFailure;
        }

        var target = resolved.Target!;
        target.PasswordHash = _hasher.Hash(temporaryPassword!);
        target.MustChangePassword = true;
        await _db.SaveChangesAsync(cancellationToken);
        return CommandResult.Success(targetId);
    }

    private async Task<(CommandResult? Failure, User? Target, string[] ActorRoles)> ResolveTargetAsync(
        int actorId,
        int targetId,
        CancellationToken cancellationToken)
    {
        var actorRoles = await UserQueries.GetRolesAsync(_db, actorId, cancellationToken);
        var target = await _db.Users
            .Include(user => user.Roles)
            .FirstOrDefaultAsync(user => user.Id == targetId, cancellationToken);

        if (target is null)
        {
            return (CommandResult.NotFound(), null, actorRoles);
        }

        var targetRoles = target.Roles.Select(role => role.Role).ToList();
        return ManagementScope.CanManage(actorRoles, targetRoles)
            ? (null, target, actorRoles)
            : (CommandResult.Forbidden(), null, actorRoles);
    }

    private async Task<CommandResult?> CheckPinAsync(int actorId, string? pin, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(pin))
        {
            return CommandResult.Rejected("Pin", "PIN girin.");
        }

        var status = await _pins.VerifyAsync(actorId, pin, cancellationToken);
        return status == PinCheckStatus.Accepted ? null : CommandResult.FromPin(status, "Pin");
    }

    private static string CheckFullName(string? value, List<FieldError> errors)
    {
        var fullName = value?.Trim() ?? string.Empty;
        if (fullName.Length == 0)
        {
            errors.Add(new FieldError("FullName", "Ad soyad girin."));
        }
        else if (fullName.Length > FullNameMaxLength)
        {
            errors.Add(new FieldError("FullName", $"Ad soyad en çok {FullNameMaxLength} karakter olabilir."));
        }

        return fullName;
    }

    private static string[] CheckRoles(string[]? selected, string[] assignable, List<FieldError> errors)
    {
        var roles = (selected ?? []).Distinct().ToArray();
        if (roles.Length == 0)
        {
            errors.Add(new FieldError("SelectedRoles", "En az bir rol seçin."));
        }
        else if (roles.Any(role => !assignable.Contains(role)))
        {
            errors.Add(new FieldError("SelectedRoles", "Seçilen rolü atayamazsınız."));
        }

        return roles;
    }
}
