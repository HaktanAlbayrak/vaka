using Microsoft.EntityFrameworkCore;
using Vaka.Web.Authorization;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

public sealed class UserAdministration : IUserAdministration
{
    private const int RegistryNoMaxLength = 32;
    private const int FullNameMaxLength = 120;
    private const string DuplicateRegistryNo = "Bu sicil no kayıtlı.";

    private readonly VakaDbContext _db;
    private readonly ICredentialHasher _hasher;
    private readonly IPinService _pins;
    private readonly IWriteTransactionRunner _transactions;
    private readonly IOpenWorkProvider _openWork;
    private readonly TimeProvider _time;

    public UserAdministration(
        VakaDbContext db,
        ICredentialHasher hasher,
        IPinService pins,
        IWriteTransactionRunner transactions,
        IOpenWorkProvider openWork,
        TimeProvider time)
    {
        _db = db;
        _hasher = hasher;
        _pins = pins;
        _transactions = transactions;
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
        return await RunExclusiveAsync(actorId, targetId, async (target, _) =>
        {
            var block = await GetDeactivateBlockAsync(actorId, targetId, target, cancellationToken);
            if (block is not null)
            {
                return CommandResult.Rejected(string.Empty, block);
            }

            var pinFailure = await _pins.ConfirmAsync(actorId, pin, cancellationToken);
            if (pinFailure is not null)
            {
                return pinFailure;
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
    /// PL-29, F1-A2: aktif Süper admin sayısını azaltabilen işlemler ve PIN isteyen her işlem başta yazma kilidi alan tek işlemde çalışır
    /// (<see cref="IWriteTransactionRunner"/>). İşlemi yapan kişi, hedef ve roller kilitten sonra veritabanından yeniden okunur; işlemi yapan
    /// artık aktif değilse ya da kapsamı kalmadıysa değişiklik yapılmaz. PIN doğrulaması iş kuralları geçtikten sonra, iş eyleminden önce
    /// aynı işlemde yapılır: yanlış PIN'de yalnız hatalı deneme sayacı kalır, doğru PIN'den sonra iş reddinde hiçbir yazma kalmaz.
    /// </summary>
    private Task<CommandResult> RunExclusiveAsync(
        int actorId,
        int targetId,
        Func<User, string[], Task<CommandResult>> action,
        CancellationToken cancellationToken) =>
        _transactions.RunAsync(async token =>
        {
            if (!await UserQueries.IsActiveAsync(_db, actorId, token))
            {
                return CommandResult.Forbidden();
            }

            var resolved = await ResolveTargetAsync(actorId, targetId, token);
            return resolved.Failure ?? await action(resolved.Target!, resolved.ActorRoles);
        }, cancellationToken);

    public async Task<CommandResult> ActivateAsync(int actorId, int targetId, string? pin, CancellationToken cancellationToken)
    {
        return await RunExclusiveAsync(actorId, targetId, async (target, _) =>
        {
            if (target.IsActive)
            {
                return CommandResult.Rejected(string.Empty, "Hesap zaten etkin.");
            }

            var pinFailure = await _pins.ConfirmAsync(actorId, pin, cancellationToken);
            if (pinFailure is not null)
            {
                return pinFailure;
            }

            target.IsActive = true;
            await _db.SaveChangesAsync(cancellationToken);
            return CommandResult.Success(targetId);
        }, cancellationToken);
    }

    public async Task<CommandResult> ResetPinAsync(int actorId, int targetId, string? pin, CancellationToken cancellationToken)
    {
        return await RunExclusiveAsync(actorId, targetId, async (_, _) =>
        {
            var pinFailure = await _pins.ConfirmAsync(actorId, pin, cancellationToken);
            if (pinFailure is not null)
            {
                return pinFailure;
            }

            await _pins.ResetAsync(targetId, cancellationToken);
            return CommandResult.Success(targetId);
        }, cancellationToken);
    }

    public async Task<CommandResult> ResetPasswordAsync(
        int actorId,
        int targetId,
        string? temporaryPassword,
        string? pin,
        CancellationToken cancellationToken)
    {
        return await RunExclusiveAsync(actorId, targetId, async (target, _) =>
        {
            var passwordError = PasswordRules.Validate(temporaryPassword);
            if (passwordError is not null)
            {
                return CommandResult.Rejected("TemporaryPassword", passwordError);
            }

            var pinFailure = await _pins.ConfirmAsync(actorId, pin, cancellationToken);
            if (pinFailure is not null)
            {
                return pinFailure;
            }

            target.PasswordHash = _hasher.Hash(temporaryPassword!);
            target.MustChangePassword = true;
            await _db.SaveChangesAsync(cancellationToken);
            return CommandResult.Success(targetId);
        }, cancellationToken);
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
