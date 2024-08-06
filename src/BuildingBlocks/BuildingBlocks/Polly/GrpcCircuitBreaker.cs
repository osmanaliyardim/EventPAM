using Ardalis.GuardClauses;
using global::Polly;
using Microsoft.Extensions.DependencyInjection;
using EventPAM.BuildingBlocks.Web;

namespace EventPAM.BuildingBlocks.Polly;

public static class GrpcCircuitBreaker
{
    public static IHttpClientBuilder AddGrpcCircuitBreakerPolicyHandler(this IHttpClientBuilder httpClientBuilder)
    {
        return httpClientBuilder.AddPolicyHandler((sp, _) =>
        {
            var options = sp.GetRequiredService<IConfiguration>().GetOptions<PolicyOptions>(nameof(PolicyOptions));

            Guard.Against.Null(options, nameof(options));

            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(Configs.POLLY_GRPC_CB_LOGGER);

            return Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: options.CircuitBreaker.RetryCount,
                    durationOfBreak: TimeSpan.FromSeconds(options.CircuitBreaker.BreakDuration),
                    onBreak: (response, breakDuration) =>
                    {
                        if (response?.Exception != null)
                        {
                            logger.LogError(response.Exception,
                                Messages.POLLY_SERVICE_DOWN,
                                breakDuration,
                                options.CircuitBreaker.RetryCount);
                        }
                    },
                    onReset: () =>
                    {
                        logger.LogInformation(Messages.SERVICE_RESTARTED);
                    });
        });
    }
}
