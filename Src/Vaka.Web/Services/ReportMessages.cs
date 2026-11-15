namespace Vaka.Web.Services;

public static class ReportMessages
{
    public const string InvalidRange = "Bitiş tarihi başlangıç tarihinden önce olamaz.";

    public const string UnknownFacility = "Seçilen tesis bulunamadı.";

    public const string UnknownLine = "Seçilen hat bulunamadı ya da tesise ait değil.";

    public const string UnknownMachine = "Seçilen makine bulunamadı ya da hatta ait değil.";

    public const string UnknownCleaningType = "Seçilen temizlik tipi bulunamadı.";

    public const string UnknownLot = "Seçilen lot bulunamadı.";

    public const string UnknownMode = "Sorgu türü geçersiz.";

    public const string InvalidFilter = "Süzgeç değerlerinden biri okunamadı. Tarih ve seçimleri kontrol edin.";
}
