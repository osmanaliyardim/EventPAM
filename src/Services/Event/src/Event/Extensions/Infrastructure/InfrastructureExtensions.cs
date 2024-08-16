using System.Threading.RateLimiting;
using EventPAM.BuildingBlocks.Core;
using EventPAM.BuildingBlocks.EFCore;
using EventPAM.BuildingBlocks.HealthCheck;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Logging.Serilog;
using EventPAM.BuildingBlocks.Mapster;
using EventPAM.BuildingBlocks.OpenTelemetry;
using EventPAM.BuildingBlocks.PersistMessageProcessor;
using EventPAM.BuildingBlocks.Swagger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Figgle;
using Serilog;
using EventPAM.BuildingBlocks.MassTransit;
using EventPAM.BuildingBlocks.Mongo;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Extensions;
using System.Reflection;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Logging.Serilog.Logger;
using EventPAM.BuildingBlocks.ElasticSearch;
using EventPAM.BuildingBlocks.Mailing.MailKitImplementations;
using Microsoft.FeatureManagement;
using EventPAM.BuildingBlocks.Mailing;
using EventPAM.Event.Data.Seed;
using EventPAM.Event.GrpcServer.Services;
using EventPAM.Event.Repositories;

namespace EventPAM.Event.Extensions.Infrastructure;

public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var env = builder.Environment;
        var assembly = typeof(EventRoot).Assembly;

        builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        builder.Services.AddScoped<IEventMapper, EventMapper>();
        builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();

        builder.Services.AddScoped<IMongoDbContext, EventReadDbContext>();
        builder.Services.AddScoped<IEventRepository, EventRepository>();
        builder.Services.AddScoped<ISeatRepository, SeatRepository>();
        builder.Services.AddScoped<IVenueRepository, VenueRepository>();

        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        builder.Services.AddCustomMediatR();

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

        builder.Services.AddCustomDbContext<EventDbContext>();
        builder.Services.AddScoped<IDataSeeder, EventDataSeeder>();
        builder.Services.AddMongoDbContext<EventReadDbContext>(configuration);
        builder.Services.AddPersistMessageProcessor(env);

        builder.Services.AddSingleton<IMailService, MailKitMailService>();
        builder.Services.AddSingleton<IElasticSearch, ElasticSearchManager>();
        builder.Services.AddSingleton<LoggerServiceBase, FileLogger>();
        builder.Services.AddFeatureManagement();
        builder.Services.AddMapster();

        builder.Services.AddEndpointsApiExplorer();
        builder.AddCustomSerilog(env);
        //builder.Services.AddJwt(); ---------------------------------------------------------------------------------------------------------
        builder.Services.AddCustomSwagger(configuration, assembly);
        builder.Services.AddCustomVersioning();
        builder.Services.AddValidatorsFromAssembly(assembly);
        builder.Services.AddCustomMapster(assembly);
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddMessageBroker(configuration, env, assembly);
        builder.Services.AddCustomOpenTelemetry();
        builder.Services.AddCustomHealthCheck();

        builder.Services.AddGrpc(options =>
        {
            options.Interceptors.Add<GrpcExceptionInterceptor>();
        });

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
        app.UseMigration<EventDbContext>(env);
        app.UseCorrelationId();
        app.UseCustomHealthCheck();
        app.MapGrpcService<EventGrpcServices>();
        app.UseRateLimiter();
        app.MapGet("/", x => x.Response.WriteAsync(appOptions.Name));

        if (env.IsDevelopment())
        {
            app.UseCustomSwagger();
        }

        return app;
    }
}
