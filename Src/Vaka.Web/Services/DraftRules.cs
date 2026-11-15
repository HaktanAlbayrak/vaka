using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;
using Vaka.Web.ViewModels.Procedures;

namespace Vaka.Web.Services;

internal static class DraftRules
{
    public const int TitleMaxLength = 120;
    public const int DescriptionMaxLength = 1000;
    public const int DocumentRefMaxLength = 64;

    private const int SqliteConstraint = 19;

    /// <summary>BR-04: yayınlanmış (Yürürlükte ya da Arşiv) versiyona yazılmaz; neden metniyle reddedilir.</summary>
    public static CommandResult? CheckEditable(ProcedureVersion version) =>
        version.Status == ProcedureVersionStatus.Draft ? null : CommandResult.NotEditable(version.Id);

    public static string CheckTitle(string? value, string label, List<FieldError> errors)
    {
        var title = value?.Trim() ?? string.Empty;
        if (title.Length == 0)
        {
            errors.Add(new FieldError("Title", $"{label} girin."));
        }
        else if (title.Length > TitleMaxLength)
        {
            errors.Add(new FieldError("Title", $"{label} en çok {TitleMaxLength} karakter olabilir."));
        }

        return title;
    }

    public static string? CheckOptionalText(string? value, string field, string label, int maxLength, List<FieldError> errors)
    {
        var text = value?.Trim();
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        if (text.Length > maxLength)
        {
            errors.Add(new FieldError(field, $"{label} en çok {maxLength} karakter olabilir."));
        }

        return text;
    }

    /// <summary>
    /// PL-14, BR-50: acil tipte minimum süre girilmez; planlıda boş kalabilir (yayını engeller), dolu ise pozitif tam sayı dakikadır.
    /// Saniye = Dakika x 60; saniyeye çevrim <see cref="IDurationCalculator.MinutesToSeconds"/> içindedir.
    /// </summary>
    public static int? CheckMinimum(int? minutes, CleaningTypeKind kind, IDurationCalculator calculator, List<FieldError> errors)
    {
        const string field = "MinDurationMinutes";
        if (minutes is null)
        {
            return null;
        }

        if (kind == CleaningTypeKind.Emergency)
        {
            errors.Add(new FieldError(field, "Acil tipte faz minimum süresi girilmez."));
            return null;
        }

        if (minutes <= 0 || minutes > int.MaxValue / 60)
        {
            errors.Add(new FieldError(field, "Minimum süre pozitif bir tam sayı (dakika) olmalı."));
            return null;
        }

        return calculator.MinutesToSeconds(minutes.Value);
    }

    /// <summary>Faz, adım ve medya satırlarının sıra no'ları her işlemden sonra 1'den boşluksuz yazılır.</summary>
    public static void Renumber<T>(IReadOnlyList<T> ordered, Action<T, int> setOrderNo)
    {
        for (var index = 0; index < ordered.Count; index++)
        {
            setOrderNo(ordered[index], index + 1);
        }
    }

    /// <summary>Sınırdaki öğe istenen yöne taşınmaz ve sıra değişmez. Geçersiz yön false döner.</summary>
    public static bool TryMove<T>(List<T> ordered, int index, MoveDirection direction, Action<T, int> setOrderNo)
    {
        var target = direction switch
        {
            MoveDirection.Up => index - 1,
            MoveDirection.Down => index + 1,
            _ => -1
        };

        if (direction is not (MoveDirection.Up or MoveDirection.Down))
        {
            return false;
        }

        if (target >= 0 && target < ordered.Count)
        {
            (ordered[index], ordered[target]) = (ordered[target], ordered[index]);
        }

        Renumber(ordered, setOrderNo);
        return true;
    }

    /// <summary>
    /// Kayıt sırasında tetikleyici reddi (SQLite 19, yarış durumunda yayınlanmış versiyona yazma) nedenli sonuca çevrilir; 500 üretmez.
    /// </summary>
    public static async Task<CommandResult> SaveAsync(VakaDbContext db, int versionId, CancellationToken cancellationToken)
    {
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqliteException { SqliteErrorCode: SqliteConstraint })
        {
            return CommandResult.NotEditable(versionId);
        }

        return CommandResult.Success(versionId);
    }

    /// <summary>
    /// PL-14, FR-05: yayın engelleri. Fazsız taslak "En az bir faz gerekir"; adımsız faz "Faz n adımı yok";
    /// planlı tipte minimumu tanımsız faz "Faz n minimum süresi yok" (n = faz sıra no).
    /// </summary>
    public static string[] GetPublishBlockers(
        IDurationCalculator calculator,
        bool isPlanned,
        IReadOnlyCollection<(int OrderNo, int StepCount, int? MinDurationSeconds)> phases)
    {
        if (phases.Count == 0)
        {
            return ["En az bir faz gerekir"];
        }

        var blockers = new List<string>();
        foreach (var phase in phases.OrderBy(item => item.OrderNo))
        {
            if (phase.StepCount == 0)
            {
                blockers.Add($"Faz {phase.OrderNo} adımı yok");
            }

            if (calculator.IsMinimumMissing(isPlanned, phase.MinDurationSeconds))
            {
                blockers.Add($"Faz {phase.OrderNo} minimum süresi yok");
            }
        }

        return blockers.ToArray();
    }
}
