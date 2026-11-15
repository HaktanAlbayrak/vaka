namespace Vaka.Web.Services;

/// <summary>Olay özeti anahtarı yapılandırma anahtarı <c>EventChain:HmacKey</c> (Base64, en az 32 bayt); değer repoda ve ayar dosyasında bulunmaz (PL-48).</summary>
public static class EventChainKey
{
    public const string ConfigurationKey = "EventChain:HmacKey";
    private const int MinimumBytes = 32;

    /// <summary>Anahtar yoksa, Base64 değilse ya da kısaysa nedeni içeren <see cref="InvalidOperationException"/> atar; anahtar değeri iletiye yazılmaz.</summary>
    public static byte[] Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"{ConfigurationKey} tanımlı değil. Development'ta user-secrets, üretimde EventChain__HmacKey ortam değişkeni ile verin.");
        }

        var bytes = new byte[value.Length];
        if (!Convert.TryFromBase64String(value.Trim(), bytes, out var written))
        {
            throw new InvalidOperationException($"{ConfigurationKey} Base64 değil.");
        }

        if (written < MinimumBytes)
        {
            throw new InvalidOperationException($"{ConfigurationKey} çözüldüğünde en az {MinimumBytes} bayt olmalı.");
        }

        return bytes[..written];
    }
}
