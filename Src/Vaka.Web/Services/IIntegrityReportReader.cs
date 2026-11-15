using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public interface IIntegrityReportReader
{
    /// <summary>
    /// Doğrulama ekranı (E-21): her olayın özeti (ilk 12 hane) ve zincir sonucu, durum tablosu ile olay değeri arasındaki farklar (<see cref="IRecordIntegrity"/>).
    /// Doğrulama her çağrıda yeniden hesaplanır; zaman aşımı etkisi için kayıt zaman aşımı uygulanır.
    /// </summary>
    Task<LookupResult<IntegrityReportViewModel>> BuildAsync(int recordId, CancellationToken cancellationToken);
}
