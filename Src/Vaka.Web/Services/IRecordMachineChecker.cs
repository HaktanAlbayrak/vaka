namespace Vaka.Web.Services;

public interface IRecordMachineChecker
{
    /// <summary>
    /// Formdaki tesis, hat ve makine kimliklerini sunucuda yeniden doğrular (BR-01, BR-02): makine var, aktif ve kullanımdan kaldırılmamış,
    /// hat makineye, tesis hatta uygun ve aktif olmalı. Hata varsa <paramref name="errors"/> listesine eklenir ve boş döner.
    /// </summary>
    Task<PlannedMachine?> CheckAsync(int? facilityId, int? lineId, int? machineId, List<FieldError> errors, CancellationToken cancellationToken);
}
