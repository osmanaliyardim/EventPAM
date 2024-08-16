using Microsoft.Extensions.DependencyInjection;
using Figgle;
using Microsoft.AspNetCore.Mvc;
using System.Threading.RateLimiting;
using EventPAM.BuildingBlocks.PersistMessageProcessor;
using EventPAM.BuildingBlocks.Core;
using EventPAM.Ticketing.Data;
using EventPAM.BuildingBlocks.Mongo;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Logging.Serilog;
using EventPAM.BuildingBlocks.Swagger;
using EventPAM.BuildingBlocks.Mapster;
using EventPAM.BuildingBlocks.MassTransit;
using EventPAM.BuildingBlocks.HealthCheck;
using EventPAM.BuildingBlocks.OpenTelemetry;
using EventPAM.BuildingBlocks.EventStoreDB;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Extensions;
using Serilog;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Logging.Serilog.Logger;
using EventPAM.BuildingBlocks.ElasticSearch;
using EventPAM.BuildingBlocks.Mailing.MailKitImplementations;
using Microsoft.FeatureManagement;
using EventPAM.BuildingBlocks.Mailing;

namespace EventPAM.Ticketing.Extensions.Infrastructure;

public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var env = builder.Environment;
        var assembly = typeof(TicketingRoot).Assembly;

        builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        builder.Services.AddScoped<IEventMapper, EventMapper>();
        builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();

        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        var appOptions = builder.Services.GetOptions<AppOptions>(nameof(AppOptions));

        Console.WriteLine(FiggleFonts.Standard.Render(appOptions.Name));

        builder.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true, PermitLimit = 10, QueueLimit = 0, Window = TimeSpan.FromMinutes(1)
                    }));
        });

        builder.Services.AddPersistMessageProcessor(env);
        builder.Services.AddMongoDbContext<TicketingReadDbContext>(configuration);
        builder.Services.AddEventStore(configuration, assembly)
            .AddEventStoreDBSubscriptionToAll();

        builder.Services.AddSingleton<IMailService, MailKitMailService>();
        builder.Services.AddSingleton<IElasticSearch, ElasticSearchManager>();
        builder.Services.AddSingleton<LoggerServiceBase, FileLogger>();
        builder.Services.AddFeatureManagement();
        builder.Services.AddMapster();

        builder.Services.AddEndpointsApiExplorer();
        builder.AddCustomSerilog(env);
        //builder.Services.AddJwt(); ---------------------------------------------------------------------------------------------------------
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddCustomSwagger(configuration, assembly);
        builder.Services.AddCustomVersioning();
        builder.Services.AddCustomMediatR();
        builder.Services.AddValidatorsFromAssembly(assembly);
        builder.Services.AddProblemDetails();
        builder.Services.AddCustomMapster(assembly);
        builder.Services.AddCustomHealthCheck();
        builder.Services.AddMessageBroker(configuration, env, assembly);
        builder.Services.AddCustomOpenTelemetry();
        //builder.Services.AddTransient<AuthHeaderHandler>(); -------------------------------------------------------------------------------

        builder.Services.AddGrpcClients();

        return builder;
    }

    public static IServiceCollection AddSubClassesOfType(
        this IServiceCollection services,
        Assembly assembly,
        Type type,
        Func<IServiceCollection, Type, IServiceCollection>? addWithLifeCycle = null)
    {
        var types = assembly.GetTypes().Where(t => t.IsSubclassOf(type) && type != t).ToList();
        foreach (var item in types)
            if (addWithLifeCycle == null)
                services.AddScoped(item);
            else
                addWithLifeCycle(services, type);
        return services;
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        var env = app.Environment;
        var appOptions = app.GetOptions<AppOptions>(nameof(AppOptions));

        app.MapPrometheusScrapingEndpoint();

        if (env.IsProduction())
            app.ConfigureCustomExceptionMiddleware();

        app.UseSerilogRequestLogging();
        app.UseCorrelationId();
        app.UseCustomHealthCheck();
        app.UseRateLimiter();
        app.MapGet("/", x => x.Response.WriteAsync(appOptions.Name));

        if (env.IsDevelopment())
        {
            app.UseCustomSwagger();
        }

        return app;
    }
}
