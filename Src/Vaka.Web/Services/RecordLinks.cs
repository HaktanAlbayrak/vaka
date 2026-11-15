using Vaka.Web.Authorization;

namespace Vaka.Web.Services;

public static class RecordLinks
{
    public static string Run(int recordId) => $"/records/run/{recordId}";

    /// <summary>Bağlantı yalnız görüntüleyenin erişebildiği ekrana verilir (PL-16): yürütme ekranı Operatör'e açıktır, Faz 6 detayı gelene kadar diğer roller için bağlantı yoktur.</summary>
    public static string? For(IEnumerable<string> viewerRoles, int recordId) =>
        viewerRoles.Contains(Roles.Operator) ? Run(recordId) : null;
}
