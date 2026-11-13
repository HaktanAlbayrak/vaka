using System.Text.RegularExpressions;

namespace Vaka.Web.Routing;

public sealed partial class KebabCaseParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        var text = value?.ToString();
        return text is null ? null : WordBoundary().Replace(text, "$1-$2").ToLowerInvariant();
    }

    [GeneratedRegex("([a-z0-9])([A-Z])")]
    private static partial Regex WordBoundary();
}
