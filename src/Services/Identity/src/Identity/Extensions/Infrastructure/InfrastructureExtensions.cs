using System.Threading.RateLimiting;
using EventPAM.BuildingBlocks.Core;
using EventPAM.BuildingBlocks.EFCore;
using EventPAM.BuildingBlocks.HealthCheck;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Extensions;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Logging.Serilog;
using EventPAM.BuildingBlocks.Mapster;
using EventPAM.BuildingBlocks.OpenTelemetry;
using EventPAM.BuildingBlocks.PersistMessageProcessor;
using EventPAM.BuildingBlocks.Swagger;
using EventPAM.BuildingBlocks.Web;
using EventPAM.Identity.Data;
using EventPAM.Identity.Data.Seed;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Figgle;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Logging.Serilog.Logger;
using EventPAM.BuildingBlocks.MassTransit;
using Serilog;
using EventPAM.BuildingBlocks.ElasticSearch;
using EventPAM.BuildingBlocks.Mailing.MailKitImplementations;
using EventPAM.BuildingBlocks.Mailing;
using System.Reflection;
using Microsoft.FeatureManagement;
using EventPAM.Identity.Identity.Services.UserService;
using EventPAM.Identity.Identity.Services.AuthenticatorService;
using EventPAM.Identity.Identity.Services.AuthService;
using EventPAM.Identity.Repositories;
using EventPAM.Identity.Configs;

namespace EventPAM.Identity.Extensions.Infrastructure;

public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var env = builder.Environment;
        var assembly = typeof(IdentityRoot).Assembly;

        builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        builder.Services.AddScoped<IEventMapper, EventMapper>();
        builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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
        builder.Services.AddCustomDbContext<IdentityContext>();
        builder.Services.AddScoped<IDataSeeder, IdentityDataSeeder>();

        builder.Services.AddSingleton<IMailService, MailKitMailService>();
        builder.Services.AddSingleton<IElasticSearch, ElasticSearchManager>();
        builder.Services.AddSingleton<LoggerServiceBase, FileLogger>();
        builder.Services.AddFeatureManagement();
        builder.Services.AddMapster();

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUserOperationClaimRepository, UserOperationClaimRepository>();
        builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        builder.Services.AddScoped<IOperationClaimRepository, OperationClaimRepository>();
        builder.Services.AddScoped<IOtpAuthenticatorRepository, OtpAuthenticatorRepository>();
        builder.Services.AddScoped<IEmailAuthenticatorRepository, EmailAuthenticatorRepository>();
        builder.Services.AddScoped<IAuthService, AuthManager>();
        builder.Services.AddScoped<IAuthenticatorService, AuthenticatorManager>();
        builder.Services.AddScoped<IUserService, UserManager>();

        builder.AddCustomSerilog(env);
        builder.Services.AddCustomSwagger(configuration, assembly);
        builder.Services.AddCustomVersioning();
        builder.Services.AddCustomMediatR();
        builder.Services.AddValidatorsFromAssembly(assembly);
        builder.Services.AddCustomMapster(assembly);
        builder.Services.AddCustomHealthCheck();

        builder.Services.AddMessageBroker(configuration, env, assembly);
        builder.Services.AddCustomOpenTelemetry();

        builder.AddCustomIdentityServer();

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
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

        app.UseForwardedHeaders();

        if (env.IsProduction())
            app.ConfigureCustomExceptionMiddleware();

        app.UseSerilogRequestLogging();
        app.UseMigration<IdentityContext>(env);
        app.UseCorrelationId();
        app.UseCustomHealthCheck();
        app.UseIdentityServer();

        app.MapGet("/", x => x.Response.WriteAsync(appOptions.Name));

        if (env.IsDevelopment())
        {
            app.UseCustomSwagger();
        }

        return app;
    }
}
