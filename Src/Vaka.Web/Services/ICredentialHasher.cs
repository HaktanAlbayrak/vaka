namespace Vaka.Web.Services;

public interface ICredentialHasher
{
    string Hash(string secret);

    bool Verify(string hash, string secret);
}
