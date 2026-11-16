namespace Vaka.Web.Services;

/// <summary>Görevli durumunun saf kuralları; okuyucular ve bitirme kapısı aynı tanımı kullanır (Faz 6E).</summary>
public static class ParticipantStateRules
{
    /// <summary>
    /// Görevlinin durumu. Aralığı varsa: ayrılmışsa Ayrıldı, değilse Çalışıyor. Aralığı yoksa: Faz 6E öncesinden çalışan (<paramref name="isWorking"/>) ya da
    /// bitmiş adımın tamamında çalışmış sayılan (<paramref name="workedWholeStep"/>) kişi Çalışıyor; başlamamış adımda girişini yapmış kişi Katılacak;
    /// kalanı Girişi bekleniyor. Başlamamış adımda sahip girişsizdir ve Girişi bekleniyor durumunda görünür, adım başlayınca çalışır.
    /// </summary>
    public static ParticipantState StateOf(StepStatus stepStatus, bool hasInterval, bool left, bool isWorking, bool workedWholeStep, bool isOwner, bool confirmed)
    {
        if (hasInterval)
        {
            return left ? ParticipantState.Left : ParticipantState.Working;
        }

        if (isWorking || workedWholeStep)
        {
            return ParticipantState.Working;
        }

        return stepStatus == StepStatus.Waiting && confirmed && !isOwner ? ParticipantState.Confirmed : ParticipantState.Pending;
    }

    public static string PendingMessage(IReadOnlyCollection<string> names) =>
        $"Girişi bekleniyor: {string.Join(", ", names)}. Kişi kendi PIN'iyle katılsın ya da sahip listeden çıkarsın.";

    /// <summary>Girişini yapmamış kişinin katılmasını engelleyen neden; engel yoksa boş (BR-36, BR-49, BR-39, satır 76).</summary>
    public static string? JoinBlock(string name, bool isActive, bool pinLocked, bool onBreak, bool busyElsewhere)
    {
        if (!isActive)
        {
            return $"{name} pasif, katılamaz.";
        }

        if (pinLocked)
        {
            return $"{name} PIN'i kilitli. Yöneticiye başvurun.";
        }

        if (onBreak)
        {
            return $"{name} molada. Mola bitince katılabilir.";
        }

        return busyElsewhere ? $"{name} başka kayıtta devam eden adımda çalışıyor." : null;
    }
}
