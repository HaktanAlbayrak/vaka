namespace Vaka.Web.Services;

public interface IPinFormatValidator
{
    /// <summary>Yeni belirlenen PIN'in biçimini denetler (BR-49); uygunsa boş, değilse hata metni döner. Doğrulamada kullanılmaz.</summary>
    Task<string?> ValidateNewPinAsync(string? pin, CancellationToken cancellationToken);
}
