using Microsoft.AspNetCore.Mvc;

namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Extensions;

internal static class ProblemDetailsExtensions
{
    public static string AsJson<TProblemDetail>(this TProblemDetail details)
        where TProblemDetail : ProblemDetails => JsonSerializer.Serialize(details);
}
