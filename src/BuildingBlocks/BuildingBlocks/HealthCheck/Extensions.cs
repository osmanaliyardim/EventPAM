using EventPAM.BuildingBlocks.EFCore;
using EventPAM.BuildingBlocks.ElasticSearch.Models;
using EventPAM.BuildingBlocks.Mongo;
using EventPAM.BuildingBlocks.Web;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EventPAM.BuildingBlocks.HealthCheck;

public static class Extensions
{
    public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services)
    {
        var healthOptions = services.GetOptions<HealthOptions>(nameof(HealthOptions));

        if (!healthOptions.Enabled) return services;

        var appOptions = services.GetOptions<AppOptions>(nameof(AppOptions));
        var postgresOptions = services.GetOptions<PostgresOptions>(nameof(PostgresOptions));
        var mongoOptions = services.GetOptions<MongoOptions>(nameof(MongoOptions));
        var elasticOptions = services.GetOptions<ElasticSearchConfig>(nameof(ElasticSearchConfig));

        var healthChecksBuilder = services.AddHealthChecks()
            .AddRabbitMQ(
                rabbitConnectionString:
                $"amqp://{Configs.RABBITMQ_USERNAME}:{Configs.RABBITMQ_PASS}@{Configs.RABBITMQ_HOST}")
            // ToDo : Change Uri to ConnectionString or to any other config
            // "http://username:password@localhost:9200"
            .AddElasticsearch(Configs.ELASTIC_URL);
            //.AddElasticsearch(elasticOptions.Uri);
            //.AddElasticsearch(elasticOptions.ConnectionString);

        if (mongoOptions.ConnectionString is not null)
            healthChecksBuilder.AddMongoDb(mongoOptions.ConnectionString);

        if (postgresOptions.ConnectionString is not null)
            healthChecksBuilder.AddNpgSql(postgresOptions.ConnectionString);

        services.AddHealthChecksUI(setup =>
        {
            setup.SetEvaluationTimeInSeconds(60); // time in seconds between check
            setup.AddHealthCheckEndpoint($"Basic Health Check - {appOptions.Name}", "/health");
        }).AddInMemoryStorage();

        return services;
    }

    public static WebApplication UseCustomHealthCheck(this WebApplication app)
    {
        var healthOptions = app.Configuration.GetOptions<HealthOptions>(nameof(HealthOptions));

        if (!healthOptions.Enabled) return app;

        app.UseHealthChecks("/health",
                new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                    ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    }
                })
            .UseHealthChecksUI(options =>
            {
                options.ApiPath = "/healthcheck";
                options.UIPath = "/healthcheck-ui";
            });

        return app;
    }
}
