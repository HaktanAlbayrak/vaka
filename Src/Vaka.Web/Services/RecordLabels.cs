namespace Vaka.Web.Services;

public static class RecordLabels
{
    public static string Status(RecordStatus status) => status switch
    {
        RecordStatus.Opened => "Açıldı",
        RecordStatus.InProgress => "Devam ediyor",
        RecordStatus.OnHold => "Beklemede",
        RecordStatus.Completed => "Tamamlandı",
        RecordStatus.TimedOut => "Zaman aşımı",
        RecordStatus.Cancelled => "İptal edildi",
        RecordStatus.Aborted => "Yarıda kapatıldı",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Bilinmeyen kayıt durumu.")
    };

    public static string Phase(PhaseStatus status) => status switch
    {
        PhaseStatus.Waiting => "Bekliyor",
        PhaseStatus.InProgress => "Devam ediyor",
        PhaseStatus.Completed => "Tamamlandı",
        PhaseStatus.CompletedBelowMinimum => "Tamamlandı - süre altında",
        PhaseStatus.Interrupted => "Kesildi",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Bilinmeyen faz durumu.")
    };

    public static string Step(StepStatus status) => status switch
    {
        StepStatus.Waiting => "Bekliyor",
        StepStatus.InProgress => "Devam ediyor",
        StepStatus.Completed => "Tamamlandı",
        StepStatus.Interrupted => "Kesildi",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Bilinmeyen adım durumu.")
    };

    public static string Machine(MachineState state) => state switch
    {
        MachineState.Free => "Boş",
        MachineState.Opened => "Açıldı",
        MachineState.InProgress => "Devam ediyor",
        MachineState.OnHold => "Beklemede",
        _ => throw new ArgumentOutOfRangeException(nameof(state), state, "Bilinmeyen makine durumu.")
    };

    public static string BreakEnd(BreakEndKind? kind) => kind switch
    {
        null => "Açık",
        BreakEndKind.Person => "Kişi bitirdi",
        BreakEndKind.Manager => "Yönetici bitirdi",
        BreakEndKind.Intervention => "Müdahale ile kapandı",
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Bilinmeyen mola kapanış türü.")
    };

    public static string ActiveRecordSummary(string ownerName, RecordStatus status, DateTime openedAtLocal) =>
        $"Sahip: {ownerName}, durum: {Status(status).ToLower(TurkishCulture)}, açılış: {openedAtLocal.ToString("dd.MM.yyyy HH:mm", TurkishCulture)}";

    private static readonly System.Globalization.CultureInfo TurkishCulture = System.Globalization.CultureInfo.GetCultureInfo("tr-TR");
}
