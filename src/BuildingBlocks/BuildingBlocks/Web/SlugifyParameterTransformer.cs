using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;

namespace EventPAM.BuildingBlocks.Web;

public partial class SlugifyParameterTransformer : IOutboundParameterTransformer
{
    public string TransformOutbound(object? value)
    {
        // Slugify value
        return value is null
            ? null!
            : TransformOutBoundRegex().Replace(value.ToString() ?? string.Empty, "$1-$2").ToLower();
    }

    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex TransformOutBoundRegex();
}
