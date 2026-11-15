namespace Vaka.Web.ViewModels.Records;

public sealed class IntegrityMismatchViewModel
{
    public string Field { get; set; } = string.Empty;

    public string StoredValue { get; set; } = string.Empty;

    public string EventValue { get; set; } = string.Empty;

    /// <summary>0: alanı doğrulayacak olay günlükte bulunamadı.</summary>
    public int Sequence { get; set; }
}
