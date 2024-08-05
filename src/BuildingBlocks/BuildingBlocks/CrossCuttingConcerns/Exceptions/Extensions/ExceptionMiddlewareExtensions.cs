using Microsoft.AspNetCore.Builder;

namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
    }
}
