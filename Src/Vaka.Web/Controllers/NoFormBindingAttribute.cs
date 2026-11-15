using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Vaka.Web.Controllers;

/// <summary>
/// Hata sayfası action'ları yalnız rota değerini kullanır. Gövde sınırını aşan istek durum kodu yönlendirmesiyle buraya düştüğünde
/// form okunmaz; aksi halde değer sağlayıcılar aynı gövdeyi okumaya çalışıp sınır hatasını yeniden üretir ve sayfa çizilemez.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class NoFormBindingAttribute : Attribute, IResourceFilter
{
    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        context.ValueProviderFactories.RemoveType<FormValueProviderFactory>();
        context.ValueProviderFactories.RemoveType<JQueryFormValueProviderFactory>();
        context.ValueProviderFactories.RemoveType<FormFileValueProviderFactory>();
    }

    public void OnResourceExecuted(ResourceExecutedContext context)
    {
    }
}
