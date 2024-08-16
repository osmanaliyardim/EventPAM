using EventPAM.BuildingBlocks.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Logging.Serilog;

public static class Extensions
{
    public static WebApplicationBuilder AddCustomSerilog(this WebApplicationBuilder builder, IWebHostEnvironment env)
    {
        builder.Host.UseSerilog((context, services, loggerConfiguration) =>
        {
            var environment = Environment.GetEnvironmentVariable(Configs.ENVIRONMENT_VARIABLE);
            var appOptions = context.Configuration.GetSection(nameof(AppOptions)).Get<AppOptions>();
        });

        return builder;
    }
}
