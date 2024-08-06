using System.Net;
using Ardalis.GuardClauses;
using EventPAM.BuildingBlocks.Web;
using global::Polly;
using global::Polly.Extensions.Http;
using Microsoft.Extensions.DependencyInjection;

namespace EventPAM.BuildingBlocks.Polly;

public static class HttpClientCircuitBreaker
{
    public static IHttpClientBuilder AddHttpClientCircuitBreakerPolicyHandler(this IHttpClientBuilder httpClientBuilder)
    {
        return httpClientBuilder.AddPolicyHandler((sp, _) =>
        {
            var options = sp.GetRequiredService<IConfiguration>().GetOptions<PolicyOptions>(nameof(PolicyOptions));

            Guard.Against.Null(options, nameof(options));

            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(Configs.POLLY_HTTP_CB_LOGGER);

            return HttpPolicyExtensions.HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.BadRequest)
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
