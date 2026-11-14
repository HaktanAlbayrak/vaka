using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.ViewModels.Users;

namespace Vaka.Web.Services;

public sealed class UserDirectory : IUserDirectory
{
    private const int PageSize = 20;

    private readonly VakaDbContext _db;
    private readonly IOpenWorkProvider _openWork;

    public UserDirectory(VakaDbContext db, IOpenWorkProvider openWork)
    {
        _db = db;
        _openWork = openWork;
    }

    public async Task<UserListViewModel> ListAsync(
        int actorId,
        string? query,
        UserStatusFilter status,
        int page,
        CancellationToken cancellationToken)
    {
        var actorRoles = await UserQueries.GetRolesAsync(_db, actorId, cancellationToken);
        var users = _db.Users.AsNoTracking();

        users = status switch
        {
            UserStatusFilter.Active => users.Where(user => user.IsActive),
            UserStatusFilter.Inactive => users.Where(user => !user.IsActive),
            _ => users
        };

        query = query?.Trim();
        if (!string.IsNullOrEmpty(query))
        {
            var pattern = $"%{query}%";
            users = users.Where(user => EF.Functions.Like(user.RegistryNo, pattern) || EF.Functions.Like(user.FullName, pattern));
        }

        var total = await users.CountAsync(cancellationToken);
        page = Math.Max(page, 1);

        var rows = await users
            .OrderBy(user => user.RegistryNo)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(user => new
            {
                user.Id,
                user.RegistryNo,
                user.FullName,
                Roles = user.Roles.Select(role => role.Role).ToList(),
                user.IsActive,
                IsPinLocked = user.PinLockedAtUtc != null,
                user.MustChangePassword,
                user.MustSetPin
            })
            .ToListAsync(cancellationToken);

        return new UserListViewModel
        {
            Items = rows.Select(row => new UserListItemViewModel
            {
                Id = row.Id,
                RegistryNo = row.RegistryNo,
                FullName = row.FullName,
                Roles = [.. row.Roles],
                IsActive = row.IsActive,
                IsPinLocked = row.IsPinLocked,
                MustChangePassword = row.MustChangePassword,
                MustSetPin = row.MustSetPin,
                CanManage = ManagementScope.CanManage(actorRoles, row.Roles)
            }).ToArray(),
            Query = query,
            Status = status,
            Page = page,
            PageSize = PageSize,
            Total = total
        };
    }

    public async Task<LookupResult<UserFormViewModel>> GetFormAsync(int actorId, int? targetId, CancellationToken cancellationToken)
    {
        var actorRoles = await UserQueries.GetRolesAsync(_db, actorId, cancellationToken);
        var available = ManagementScope.AssignableRoles(actorRoles);

        if (targetId is null)
        {
            return available.Length == 0
                ? LookupResult<UserFormViewModel>.OutOfScope()
                : LookupResult<UserFormViewModel>.Found(new UserFormViewModel { AvailableRoles = available });
        }

        var target = await _db.Users
            .AsNoTracking()
            .Where(user => user.Id == targetId)
            .Select(user => new
            {
                user.Id,
                user.RegistryNo,
                user.FullName,
                Roles = user.Roles.Select(role => role.Role).ToList(),
                user.IsActive,
                IsPinLocked = user.PinLockedAtUtc != null,
                user.MustChangePassword,
                user.MustSetPin
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (target is null)
        {
            return LookupResult<UserFormViewModel>.Missing();
        }

        if (!ManagementScope.CanManage(actorRoles, target.Roles))
        {
            return LookupResult<UserFormViewModel>.OutOfScope();
        }

        var block = await UserQueries.GetDeactivateBlockAsync(_db, actorId, target.Id, target.IsActive, cancellationToken);
        return LookupResult<UserFormViewModel>.Found(new UserFormViewModel
        {
            Id = target.Id,
            RegistryNo = target.RegistryNo,
            FullName = target.FullName,
            SelectedRoles = [.. target.Roles],
            AvailableRoles = available,
            RolesLocked = actorId == target.Id,
            IsActive = target.IsActive,
            IsPinLocked = target.IsPinLocked,
            MustChangePassword = target.MustChangePassword,
            MustSetPin = target.MustSetPin,
            CanResetPin = true,
            CanResetPassword = true,
            CanDeactivate = block is null,
            CanActivate = !target.IsActive,
            DeactivateBlockReason = block
        });
    }

    public async Task<LookupResult<UserDeactivateViewModel>> GetDeactivateAsync(int actorId, int targetId, CancellationToken cancellationToken)
    {
        var actorRoles = await UserQueries.GetRolesAsync(_db, actorId, cancellationToken);
        var target = await _db.Users
            .AsNoTracking()
            .Where(user => user.Id == targetId)
            .Select(user => new
            {
                user.Id,
                user.RegistryNo,
                user.FullName,
                Roles = user.Roles.Select(role => role.Role).ToList(),
                user.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (target is null)
        {
            return LookupResult<UserDeactivateViewModel>.Missing();
        }

        if (!ManagementScope.CanManage(actorRoles, target.Roles))
        {
            return LookupResult<UserDeactivateViewModel>.OutOfScope();
        }

        var block = await UserQueries.GetDeactivateBlockAsync(_db, actorId, target.Id, target.IsActive, cancellationToken);
        var openWork = await _openWork.ListAsync(target.Id, actorId, cancellationToken);
        if (block is null && openWork.Length > 0)
        {
            block = "Kullanıcının açık işi var. Önce bu işler kapatılmalı.";
        }

        return LookupResult<UserDeactivateViewModel>.Found(new UserDeactivateViewModel
        {
            UserId = target.Id,
            FullName = target.FullName,
            RegistryNo = target.RegistryNo,
            OpenWork = openWork,
            CanDeactivate = block is null,
            BlockReason = block
        });
    }
}
