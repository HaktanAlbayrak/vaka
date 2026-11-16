using Microsoft.EntityFrameworkCore;
using Vaka.Web.Authorization;
using Vaka.Web.Data;

namespace Vaka.Web.Services;

public sealed class AccountService : IAccountService
{
    private readonly VakaDbContext _db;
    private readonly ICredentialHasher _hasher;
    private readonly IPinService _pins;
    private readonly IPinFormatValidator _pinFormat;
    private readonly IWriteTransactionRunner _transactions;
    private readonly ILoginAttemptGuard _attempts;
    private readonly ITurkeyClock _clock;

    public AccountService(VakaDbContext db, ICredentialHasher hasher, IPinService pins, IPinFormatValidator pinFormat, IWriteTransactionRunner transactions, ILoginAttemptGuard attempts, ITurkeyClock clock)
    {
        _db = db;
        _hasher = hasher;
        _pins = pins;
        _pinFormat = pinFormat;
        _transactions = transactions;
        _attempts = attempts;
        _clock = clock;
    }

    public async Task<SignInResult> SignInAsync(string registryNo, string password, CancellationToken cancellationToken)
    {
        var user = await FindActiveAsync(registryNo, cancellationToken);
        if (user is null)
        {
            _hasher.VerifyDecoy(password);
            return new SignInResult(SignInStatus.Invalid);
        }

        return await AttemptAsync(user.Id, user.PasswordHash, password, cancellationToken);
    }

    public async Task<SignInResult> SignInWithPinAsync(string registryNo, string pin, CancellationToken cancellationToken)
    {
        var user = await FindActiveAsync(registryNo, cancellationToken);
        if (user is null || !user.OperatorOnly)
        {
            _hasher.VerifyDecoy(pin);
            return new SignInResult(SignInStatus.Invalid);
        }

        if (user.MustChangePassword || user.MustSetPin || user.PinHash is null)
        {
            return new SignInResult(SignInStatus.PasswordRequired);
        }

        if (user.PinLocked)
        {
            return new SignInResult(SignInStatus.PinLocked);
        }

        return await AttemptAsync(user.Id, user.PinHash, pin, cancellationToken);
    }

    private async Task<SignInResult> AttemptAsync(int userId, string hash, string secret, CancellationToken cancellationToken)
    {
        var lockedUntil = await _attempts.GetLockedUntilAsync(userId, cancellationToken);
        if (lockedUntil is not null)
        {
            return Locked(lockedUntil.Value);
        }

        if (!_hasher.Verify(hash, secret))
        {
            var lockedNow = await _attempts.RegisterFailureAsync(userId, cancellationToken);
            return lockedNow is null ? new SignInResult(SignInStatus.Invalid) : Locked(lockedNow.Value);
        }

        await _attempts.ResetAsync(userId, cancellationToken);
        var state = await GetStateAsync(userId, cancellationToken);
        return state is null ? new SignInResult(SignInStatus.Invalid) : new SignInResult(SignInStatus.Success, State: state);
    }

    private SignInResult Locked(DateTime untilUtc) =>
        new(SignInStatus.LockedUntil, _clock.ToLocal(untilUtc));

    private async Task<SignInCandidate?> FindActiveAsync(string registryNo, CancellationToken cancellationToken)
    {
        var row = await _db.Users
            .AsNoTracking()
            .Where(candidate => candidate.RegistryNo == registryNo && candidate.IsActive)
            .Select(candidate => new
            {
                candidate.Id,
                candidate.PasswordHash,
                candidate.PinHash,
                candidate.MustChangePassword,
                candidate.MustSetPin,
                PinLocked = candidate.PinLockedAtUtc != null,
                Roles = candidate.Roles.Select(role => role.Role).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return row is null
            ? null
            : new SignInCandidate(
                row.Id,
                row.PasswordHash,
                row.PinHash,
                row.MustChangePassword,
                row.MustSetPin,
                row.PinLocked,
                row.Roles.Count > 0 && row.Roles.All(role => role == Roles.Operator));
    }

    private sealed record SignInCandidate(
        int Id,
        string PasswordHash,
        string? PinHash,
        bool MustChangePassword,
        bool MustSetPin,
        bool PinLocked,
        bool OperatorOnly);

    public async Task<AccountState?> GetStateAsync(int userId, CancellationToken cancellationToken)
    {
        var row = await _db.Users
            .AsNoTracking()
            .Where(candidate => candidate.Id == userId)
            .Select(candidate => new
            {
                candidate.Id,
                candidate.FullName,
                candidate.RegistryNo,
                Roles = candidate.Roles.Select(role => role.Role).ToList(),
                candidate.IsActive,
                candidate.MustChangePassword,
                candidate.MustSetPin
            })
            .FirstOrDefaultAsync(cancellationToken);

        return row is null
            ? null
            : new AccountState(row.Id, row.FullName, row.RegistryNo, [.. row.Roles], row.IsActive, row.MustChangePassword, row.MustSetPin);
    }

    public async Task<CommandResult> CompleteFirstLoginAsync(int userId, FirstLoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(candidate => candidate.Id == userId, cancellationToken);
        if (user is null)
        {
            return CommandResult.NotFound();
        }

        var errors = new List<FieldError>();
        if (user.MustChangePassword)
        {
            errors.AddRange(CheckPasswordChange(user.PasswordHash, request.CurrentPassword, request.NewPassword));
        }

        if (user.MustSetPin)
        {
            var pinError = await _pinFormat.ValidateNewPinAsync(request.NewPin, cancellationToken);
            if (pinError is not null)
            {
                errors.Add(new FieldError("NewPin", pinError));
            }
        }

        if (errors.Count > 0)
        {
            return CommandResult.Rejected(errors);
        }

        if (user.MustChangePassword)
        {
            user.PasswordHash = _hasher.Hash(request.NewPassword!);
            user.MustChangePassword = false;
            await _db.SaveChangesAsync(cancellationToken);
        }

        if (user.MustSetPin)
        {
            await _pins.SetAsync(userId, request.NewPin!, cancellationToken);
        }

        return CommandResult.Success();
    }

    public async Task<CommandResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(candidate => candidate.Id == userId, cancellationToken);
        if (user is null)
        {
            return CommandResult.NotFound();
        }

        var errors = CheckPasswordChange(user.PasswordHash, currentPassword, newPassword);
        if (errors.Count > 0)
        {
            return CommandResult.Rejected(errors);
        }

        user.PasswordHash = _hasher.Hash(newPassword);
        user.MustChangePassword = false;
        await _db.SaveChangesAsync(cancellationToken);
        return CommandResult.Success();
    }

    public async Task<CommandResult> ChangePinAsync(int userId, string currentPin, string newPin, CancellationToken cancellationToken) =>
        await _transactions.RunAsync(async token =>
        {
            var status = await _pins.VerifyAsync(userId, currentPin, token);
            if (status != PinCheckStatus.Accepted)
            {
                return CommandResult.FromPin(status, "CurrentPin");
            }

            var pinError = await _pinFormat.ValidateNewPinAsync(newPin, token);
            if (pinError is not null)
            {
                return CommandResult.Rejected("NewPin", pinError);
            }

            await _pins.SetAsync(userId, newPin, token);
            return CommandResult.Success();
        }, cancellationToken);

    private List<FieldError> CheckPasswordChange(string passwordHash, string? currentPassword, string? newPassword)
    {
        var errors = new List<FieldError>();
        if (string.IsNullOrEmpty(currentPassword) || !_hasher.Verify(passwordHash, currentPassword))
        {
            errors.Add(new FieldError("CurrentPassword", "Mevcut parola hatalı."));
        }

        var ruleError = PasswordRules.Validate(newPassword);
        if (ruleError is not null)
        {
            errors.Add(new FieldError("NewPassword", ruleError));
        }
        else if (newPassword == currentPassword)
        {
            errors.Add(new FieldError("NewPassword", "Yeni parola mevcut parolayla aynı olamaz."));
        }

        return errors;
    }
}
