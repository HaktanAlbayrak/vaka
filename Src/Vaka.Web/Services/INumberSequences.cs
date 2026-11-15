namespace Vaka.Web.Services;

public interface INumberSequences
{
    /// <summary>
    /// Kapsam ve yıl için sıradaki numarayı verir ve kaydeder: sıra = son değer + 1, satır yoksa 1 (BR-08, BR-09).
    /// Sayaç yalnız UPDATE ile artar (tetikleyici); yazma kilitli işlemin içinde çağrılmalıdır, işlem geri alınırsa sayaç da geri döner.
    /// </summary>
    Task<int> NextAsync(string scope, int year, CancellationToken cancellationToken);
}
