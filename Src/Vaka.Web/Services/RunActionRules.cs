using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

/// <summary>Yürütme ekranının eylem bayraklarının nedenleri. Yalnız görünümü yönlendirir; asıl denetim adım, görevli ve mola servislerindedir.</summary>
public static class RunActionRules
{
    public static string? StartBlock(RunContext context, bool openBreak, bool isNext, bool anyInProgress, bool isMember, bool missingMaterial)
    {
        if (context.Status is not (RecordStatus.Opened or RecordStatus.InProgress))
        {
            return context.Status == RecordStatus.TimedOut ? "Kilit süresi doldu" : "Kayıt bu durumda adım başlatılamaz";
        }

        if (openBreak)
        {
            return "Kayıtta açık mola var";
        }

        if (!isNext)
        {
            return anyInProgress ? "Devam eden adım bitmeden başlatılamaz" : "Önceki adım tamamlanmadan başlatılamaz";
        }

        if (!isMember)
        {
            return "Bu adımın görevlisi değilsiniz";
        }

        return missingMaterial ? "Malzeme zorunlu: önce kayda malzeme ekleyin" : null;
    }

    public static string? FinishBlock(RunContext context, bool openBreak, bool isMember)
    {
        if (context.Status != RecordStatus.InProgress)
        {
            return "Kayıt bu durumda adım bitirilemez";
        }

        if (openBreak)
        {
            return "Kayıtta açık mola var";
        }

        return isMember ? null : "Bu adımın görevlisi değilsiniz";
    }

    /// <summary>Mola yalnız kayıt Devam ediyor ve devam eden adım yokken, sahip ya da sıradaki adımın görevlisi için (BR-38, SK-6).</summary>
    public static string? BreakBlock(RunContext context, bool openBreak, bool anyInProgress, RecordStepViewModel? next)
    {
        if (context.Status == RecordStatus.Completed)
        {
            return null;
        }

        if (openBreak)
        {
            return "Kayıtta açık mola var";
        }

        if (context.Status == RecordStatus.Opened)
        {
            return "Mola ilk adım başladıktan sonra, iki adım arasında verilir";
        }

        if (context.Status != RecordStatus.InProgress)
        {
            return "Kayıt bu durumda mola verilemez";
        }

        if (anyInProgress)
        {
            return "Mola iki adım arasında verilir";
        }

        var isMember = context.OwnerUserId == context.ViewerUserId || (next?.Participants.Any(item => item.UserId == context.ViewerUserId) ?? false);
        return isMember ? null : "Mola vermek için kayıt sahibi ya da sıradaki adımın görevlisi olmalısınız";
    }

    public static bool CanStartBreak(RunContext context, bool openBreak, bool anyInProgress, RecordStepViewModel? next) =>
        context.Status == RecordStatus.InProgress && BreakBlock(context, openBreak, anyInProgress, next) is null;
}
