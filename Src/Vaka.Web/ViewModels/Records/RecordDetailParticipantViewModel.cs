namespace Vaka.Web.ViewModels.Records;

public sealed class RecordDetailParticipantViewModel
{
    public string FullName { get; set; } = string.Empty;

    public string RegistryNo { get; set; } = string.Empty;

    public string AddedByName { get; set; } = string.Empty;

    public DateTime AddedAtLocal { get; set; }

    /// <summary>Görevlinin PIN onayı Faz 7'de gelir (PL-57); yalnız onay anı kayıtlıysa dolar.</summary>
    public DateTime? ConfirmedAtLocal { get; set; }

    public DateTime? JoinedAtLocal { get; set; }

    public DateTime? LeftAtLocal { get; set; }

    public int? WorkSeconds { get; set; }

    /// <summary>Aralık satırı var; yoksa (Faz 6E öncesi adım) görünüm "Adım boyunca" yazar.</summary>
    public bool HasInterval { get; set; }
}
