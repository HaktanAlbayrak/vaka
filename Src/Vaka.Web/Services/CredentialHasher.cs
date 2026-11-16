using Microsoft.AspNetCore.Identity;

namespace Vaka.Web.Services;

public sealed class CredentialHasher : ICredentialHasher
{
    private readonly PasswordHasher<string> _hasher = new();
    private readonly string _decoyHash;

    public CredentialHasher()
    {
        _decoyHash = Hash(Guid.NewGuid().ToString("N"));
    }

    public string Hash(string secret) => _hasher.HashPassword(string.Empty, secret);

    public bool Verify(string hash, string secret) =>
        _hasher.VerifyHashedPassword(string.Empty, hash, secret) != PasswordVerificationResult.Failed;

    public void VerifyDecoy(string secret) => _hasher.VerifyHashedPassword(string.Empty, _decoyHash, secret);
}
