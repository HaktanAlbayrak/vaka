using Vaka.Web.Authorization;

namespace Vaka.Web.Services;

public static class ManagementScope
{
    private static readonly string[] ManagerAssignable = [Roles.Operator, Roles.Auditor];

    private static readonly string[] SuperAdminAssignable = [Roles.Operator, Roles.Manager, Roles.Auditor, Roles.SuperAdmin];

    /// <summary>SK-9: Süper admin tüm hesapları; Yönetici yalnız Manager ve SuperAdmin rolü taşımayan hesapları yönetir.</summary>
    public static bool CanManage(IReadOnlyCollection<string> actorRoles, IReadOnlyCollection<string> targetRoles)
    {
        if (actorRoles.Contains(Roles.SuperAdmin))
        {
            return true;
        }

        return actorRoles.Contains(Roles.Manager)
            && !targetRoles.Contains(Roles.Manager)
            && !targetRoles.Contains(Roles.SuperAdmin);
    }

    public static string[] AssignableRoles(IReadOnlyCollection<string> actorRoles)
    {
        if (actorRoles.Contains(Roles.SuperAdmin))
        {
            return SuperAdminAssignable;
        }

        return actorRoles.Contains(Roles.Manager) ? ManagerAssignable : [];
    }
}
