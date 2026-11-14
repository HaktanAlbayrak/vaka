namespace Vaka.Web.Services;

public static class PasswordRules
{
    private const int MinimumLength = 6;

    /// <summary>ASP.NET Identity varsayılanları: en az 6 karakter, rakam, küçük harf, büyük harf, alfasayısal olmayan karakter.</summary>
    public static string? Validate(string? password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return "Parola girin.";
        }

        if (password.Length < MinimumLength)
        {
            return $"Parola en az {MinimumLength} karakter olmalı.";
        }

        if (!password.Any(char.IsDigit))
        {
            return "Parola en az bir rakam içermeli.";
        }

        if (!password.Any(char.IsLower))
        {
            return "Parola en az bir küçük harf içermeli.";
        }

        if (!password.Any(char.IsUpper))
        {
            return "Parola en az bir büyük harf içermeli.";
        }

        if (password.All(char.IsLetterOrDigit))
        {
            return "Parola en az bir özel karakter içermeli.";
        }

        return null;
    }
}
