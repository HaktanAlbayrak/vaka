namespace Vaka.Web.Services;

internal static class DefinitionRules
{
    public const int CodeMaxLength = 16;
    public const int MaterialCodeMaxLength = 32;
    public const int NameMaxLength = 120;
    public const int ShiftNameMaxLength = 80;
    public const int LotNoMaxLength = 64;

    /// <summary>BR-54: tesis, hat ve makine kodu kayıt numarasında ayırıcı olan tireyi içeremez; <paramref name="allowDash"/> malzeme kodu ve lot no içindir.</summary>
    public static string CheckCode(string? value, string label, int maxLength, bool allowDash, List<FieldError> errors, string field = "Code")
    {
        var code = value?.Trim() ?? string.Empty;
        if (code.Length == 0)
        {
            errors.Add(new FieldError(field, $"{label} girin."));
        }
        else if (code.Length > maxLength)
        {
            errors.Add(new FieldError(field, $"{label} en çok {maxLength} karakter olabilir."));
        }
        else if (!allowDash && code.Contains('-'))
        {
            errors.Add(new FieldError(field, $"{label} tire içeremez."));
        }

        return code;
    }

    public static string CheckName(string? value, string label, int maxLength, List<FieldError> errors)
    {
        var name = value?.Trim() ?? string.Empty;
        if (name.Length == 0)
        {
            errors.Add(new FieldError("Name", $"{label} girin."));
        }
        else if (name.Length > maxLength)
        {
            errors.Add(new FieldError("Name", $"{label} en çok {maxLength} karakter olabilir."));
        }

        return name;
    }

    /// <summary>PL-04: yeni kod boşsa form alanı gönderilmemiş sayılır; dolu ve kayıtlı koddan farklıysa değişiklik denemesidir.</summary>
    public static bool IsCodeChange(string? posted, string stored)
    {
        var code = posted?.Trim();
        return !string.IsNullOrEmpty(code) && !string.Equals(code, stored, StringComparison.Ordinal);
    }
}
