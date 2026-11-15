using Vaka.Web.Authorization;

namespace Vaka.Web.Services;

public static class RecordLinks
{
    public static string Run(int recordId) => $"/records/run/{recordId}";

    public static string Details(int recordId) => $"/records/details/{recordId}";

    public static string Integrity(int recordId) => $"/records/integrity/{recordId}";

    public static string Intervene(int recordId) => $"/interventions/create?recordId={recordId}";

    public static string EndBreak(int breakId) => $"/interventions/end-break/{breakId}";

    /// <summary>
    /// Bağlantı yalnız görüntüleyenin erişebildiği ekrana verilir (PL-16): Yönetici ve Denetçi kayıt detayına, yalnız Operatör olan yürütme ekranına gider;
    /// başka rolde bağlantı yoktur.
    /// </summary>
    public static string? For(IEnumerable<string> viewerRoles, int recordId)
    {
        var roles = viewerRoles.ToArray();
        if (roles.Contains(Roles.Manager) || roles.Contains(Roles.Auditor))
        {
            return Details(recordId);
        }

        return roles.Contains(Roles.Operator) ? Run(recordId) : null;
    }
}
