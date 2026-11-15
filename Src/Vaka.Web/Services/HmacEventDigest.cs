using System.Security.Cryptography;
using System.Text;

namespace Vaka.Web.Services;

public sealed class HmacEventDigest : IEventDigest
{
    private readonly byte[] _key;

    public HmacEventDigest(byte[] key)
    {
        _key = key;
    }

    public string Compute(string? previousHash, string canonicalEvent)
    {
        var input = Encoding.UTF8.GetBytes($"{previousHash}\n{canonicalEvent}");
        return Convert.ToHexStringLower(HMACSHA256.HashData(_key, input));
    }
}
