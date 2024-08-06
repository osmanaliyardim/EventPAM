using System.Net;
using Ardalis.GuardClauses;
using EventPAM.BuildingBlocks.Web;
using global::Polly;
using global::Polly.Extensions.Http;
using Microsoft.Extensions.DependencyInjection;

namespace EventPAM.BuildingBlocks.Polly;

public static class HttpClientRetry
{
    public static IHttpClientBuilder AddHttpClientRetryPolicyHandler(this IHttpClientBuilder httpClientBuilder)
    {
        return httpClientBuilder.AddPolicyHandler((sp, _) =>
        {
            var options = sp.GetRequiredService<IConfiguration>().GetOptions<PolicyOptions>(nameof(PolicyOptions));

            Guard.Against.Null(options, nameof(options));

            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(Configs.POLLY_HTTP_CB_LOGGER);

            return HttpPolicyExtensions.HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.BadRequest)
                .OrResult(msg => msg.StatusCode == HttpStatusCode.InternalServerError)
                .WaitAndRetryAsync(retryCount: options.Retry.RetryCount,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(options.Retry.SleepDuration),
                    onRetry: (response, timeSpan, retryCount, context) =>
                    {
                        if (response?.Exception != null)
                        {
                            logger.LogError(response.Exception,
                                Messages.POLLY_FAILED,
                                response.Result.StatusCode,
                                timeSpan,
                                retryCount);
                        }
                    });
        });
    }
}
