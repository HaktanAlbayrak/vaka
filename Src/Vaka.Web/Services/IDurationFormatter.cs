namespace Vaka.Web.Services;

public interface IDurationFormatter
{
    /// <summary>
    /// Saniyeyi "sa dk sn" parçalarına böler (T-B14); görünüm rakamı ve birimi ayrı yazar.
    /// sa = saniye / 3600; dk = (saniye % 3600) / 60; sn = saniye % 60 (tam bölme)
    /// Sıfır olan birim atlanır; toplam sıfırsa tek parça <c>0 sn</c> döner.
    /// </summary>
    /// <remarks>Negatif saniye geçersizdir. Yalnız aritmetik yapar, süre hakkında karar vermez.</remarks>
    DurationPart[] Format(int seconds);
}
