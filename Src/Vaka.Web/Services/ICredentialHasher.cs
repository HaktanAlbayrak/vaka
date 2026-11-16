namespace Vaka.Web.Services;

public interface ICredentialHasher
{
    string Hash(string secret);

    bool Verify(string hash, string secret);

    /// <summary>Hesap bulunamadığında doğrulama süresini eşitlemek için sahte özete karşı aynı maliyette doğrulama yapar; sonuç hep ret.</summary>
    void VerifyDecoy(string secret);
}
