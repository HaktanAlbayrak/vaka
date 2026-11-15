namespace Vaka.Web.Services;

public interface IShiftLocator
{
    /// <summary>Tesisin aktif vardiyalarını okuyup anı içereni bulur (PL-40, K-38); hiçbiri içermiyorsa ya da vardiya yoksa boş döner.</summary>
    Task<ShiftOccurrence?> FindAtAsync(int facilityId, DateTimeOffset moment, CancellationToken cancellationToken);
}
