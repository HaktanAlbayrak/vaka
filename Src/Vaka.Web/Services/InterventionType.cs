namespace Vaka.Web.Services;

/// <summary>Yöneticinin kayda doğrudan müdahalesi (BR-27). Değer kayıtta <c>ClosureKind</c> olarak saklanır. Beklemeye alma Faz 7'de eklenir.</summary>
public enum InterventionType
{
    Cancel = 1,
    Abort = 2
}
