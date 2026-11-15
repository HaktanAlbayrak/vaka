using Microsoft.AspNetCore.Mvc.Razor;

namespace Vaka.Web.Routing;

/// <summary>
/// <c>RecordSteps</c> ve <c>Breaks</c> controller'ları reddedilen istekte <c>Views/Records</c> görünümlerini (Run, FinishStep) ve kısmi görünümlerini yeniden çizer;
/// konum yalnız bu iki controller için eklenir, diğerlerinde eksik görünüm başka klasörden bulunmaz.
/// </summary>
public sealed class RecordViewLocationExpander : IViewLocationExpander
{
    private const string ControllerKey = "controller";

    public void PopulateValues(ViewLocationExpanderContext context) =>
        context.Values[ControllerKey] = context.ActionContext.ActionDescriptor.RouteValues.TryGetValue("controller", out var name) ? name : null;

    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations) =>
        context.Values.TryGetValue(ControllerKey, out var controller) && controller is "RecordSteps" or "Breaks"
            ? viewLocations.Append("/Views/Records/{0}.cshtml")
            : viewLocations;
}
