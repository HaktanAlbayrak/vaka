using Vaka.Web.ViewModels.Interventions;

namespace Vaka.Web.Services;

public interface IInterventionForms
{
    /// <summary>
    /// Müdahale formu (E-26). Önce kayıt için zaman aşımı uygulanır. İzinli türler kayıt durumundan gelir (<see cref="InterventionRules.AllowedTypes"/>);
    /// izinli tür yoksa <c>BlockReason</c> dolar. <paramref name="type"/> izinliyse seçili gelir. <c>CommandId</c> her çizimde yenidir.
    /// </summary>
    Task<LookupResult<InterventionViewModel>> BuildCreateAsync(int recordId, InterventionType? type, CancellationToken cancellationToken);

    /// <summary>Yöneticinin mola bitirme formu. Mola bitmişse <c>BlockReason</c> dolar. <c>CommandId</c> her çizimde yenidir.</summary>
    Task<LookupResult<EndBreakViewModel>> BuildEndBreakAsync(int breakId, CancellationToken cancellationToken);
}
