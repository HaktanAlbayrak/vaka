using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Services;

public static class ProcedureLabels
{
    public const string SetupData = "Kurulum verisi";

    public static string Kind(CleaningTypeKind kind) => kind switch
    {
        CleaningTypeKind.Planned => "Planlı",
        CleaningTypeKind.Emergency => "Acil",
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Bilinmeyen temizlik tipi türü.")
    };

    public static string Status(ProcedureVersionStatus status) => status switch
    {
        ProcedureVersionStatus.Draft => "Taslak",
        ProcedureVersionStatus.Current => "Yürürlükte",
        ProcedureVersionStatus.Archived => "Arşiv",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Bilinmeyen versiyon durumu.")
    };

    public static string MediaType(StepMediaType type) => type switch
    {
        StepMediaType.Photo => "Fotoğraf",
        StepMediaType.Video => "Video",
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Bilinmeyen medya türü.")
    };

    public static SelectOptionViewModel[] KindOptions() =>
    [
        new SelectOptionViewModel { Id = (int)CleaningTypeKind.Planned, Label = Kind(CleaningTypeKind.Planned) },
        new SelectOptionViewModel { Id = (int)CleaningTypeKind.Emergency, Label = Kind(CleaningTypeKind.Emergency) }
    ];
}
