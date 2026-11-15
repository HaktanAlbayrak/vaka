using System.Globalization;
using System.Text.RegularExpressions;

namespace Vaka.Web.Services;

public static partial class RecordNumbers
{
    public static string RecordScope(int machineId) => $"Record:{machineId.ToString(CultureInfo.InvariantCulture)}";

    /// <summary>Kayıt no = &lt;TesisKodu&gt;-&lt;HatKodu&gt;-&lt;MakineKodu&gt;-&lt;Tür&gt;&lt;Yıl&gt;-&lt;Sıra&gt;; tür planlı için P, acil için A; sıfır dolgusu yok (BR-08).</summary>
    public static string Format(string facilityCode, string lineCode, string machineCode, CleaningTypeKind kind, int year, int sequence)
    {
        var letter = kind == CleaningTypeKind.Planned ? 'P' : 'A';
        return string.Create(CultureInfo.InvariantCulture, $"{facilityCode}-{lineCode}-{machineCode}-{letter}{year}-{sequence}");
    }

    public static string FieldBookScope(int facilityId) => $"FieldBook:{facilityId.ToString(CultureInfo.InvariantCulture)}";

    /// <summary>Saha referansı = &lt;TesisKodu&gt;-SD-&lt;Yıl&gt;-&lt;Sıra&gt;; sıra tesis ve yıl başına artar, sıfır dolgusu yok (BR-09).</summary>
    public static string FormatFieldBook(string facilityCode, int year, int sequence) =>
        string.Create(CultureInfo.InvariantCulture, $"{facilityCode}-SD-{year}-{sequence}");

    public static string WorkOrderScope(int facilityId) => $"WorkOrder:{facilityId.ToString(CultureInfo.InvariantCulture)}";

    /// <summary>Sistem iş emri no = &lt;TesisKodu&gt;-IE-&lt;Yıl&gt;-&lt;Sıra&gt;; sıra tesis ve yıl başına artar, sıfır dolgusu yok (PL-80).</summary>
    public static string FormatWorkOrder(string facilityCode, int year, int sequence) =>
        string.Create(CultureInfo.InvariantCulture, $"{facilityCode}-IE-{year}-{sequence}");

    /// <summary>Elle girilen değer sistemin atadığı biçime uyuyorsa true; büyük küçük harf ayrımı yoktur, böylece elle değer sistem numarasıyla çakışamaz.</summary>
    public static bool LooksLikeSystemWorkOrder(string value) => SystemWorkOrderPattern().IsMatch(value);

    [GeneratedRegex(@"^.+-IE-[0-9]{4}-[0-9]+$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex SystemWorkOrderPattern();
}
