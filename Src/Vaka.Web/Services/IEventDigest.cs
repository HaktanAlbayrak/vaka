namespace Vaka.Web.Services;

public interface IEventDigest
{
    /// <summary>Özet = HMAC-SHA256(anahtar, önceki özet + "\n" + kanonik olay), küçük harf onaltılık (BR-32). İlk olayda önceki özet boş metindir.</summary>
    string Compute(string? previousHash, string canonicalEvent);
}
