using EventPAM.BuildingBlocks.PersistMessageProcessor.Data;
using EventPAM.BuildingBlocks.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace EventPAM.BuildingBlocks.PersistMessageProcessor;

public static class Extensions
{
    public static IServiceCollection AddPersistMessageProcessor(this IServiceCollection services,
        IWebHostEnvironment env)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddValidateOptions<PersistMessageOptions>();

        services.AddDbContext<PersistMessageDbContext>((sp, options) =>
        {
            var persistMessageOptions = sp.GetRequiredService<PersistMessageOptions>();

            options.UseNpgsql(persistMessageOptions.ConnectionString,
                    dbOptions =>
                    {
                        dbOptions.MigrationsAssembly(typeof(PersistMessageDbContext).Assembly.GetName().Name);
                    })
                .UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IPersistMessageDbContext>(provider =>
        {
            var persistMessageDbContext = provider.GetRequiredService<PersistMessageDbContext>();

            persistMessageDbContext.Database.EnsureCreated();
            persistMessageDbContext.CreatePersistMessageTable();

            return persistMessageDbContext;
        });

        services.AddScoped<IPersistMessageProcessor, PersistMessageProcessor>();

        if (env.EnvironmentName != "test")
        {
            services.AddHostedService<PersistMessageBackgroundService>();
        }

        return services;
    }
}
