namespace Vaka.Web.Authorization;

public static class SessionEndReason
{
    public const string QueryKey = "reason";
    public const string ItemKey = "Vaka.SessionEndReason";

    public const string Expired = "expired";
    public const string Inactive = "inactive";
    public const string PinLocked = "pin-locked";
    public const string SignedOut = "signed-out";

    public static string? Message(string? reason) => reason switch
    {
        Expired => "Oturum süresi doldu. Yeniden giriş yapın.",
        Inactive => "Hesap pasif. Oturum kapatıldı.",
        PinLocked => "PIN kilitlendi ve oturum kapatıldı. PIN'inizi sıfırlatmak için yöneticinize başvurun.",
        SignedOut => "Oturum kapatıldı.",
        _ => null
    };
}
