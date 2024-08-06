using Ardalis.GuardClauses;
using EventPAM.BuildingBlocks.Web;
using global::Polly;
using Microsoft.Extensions.DependencyInjection;

namespace EventPAM.BuildingBlocks.Polly;

public static class GrpcRetry
{
    public static IHttpClientBuilder AddGrpcRetryPolicyHandler(this IHttpClientBuilder httpClientBuilder)
    {
        return httpClientBuilder.AddPolicyHandler((sp, _) =>
        {
            var options = sp.GetRequiredService<IConfiguration>().GetOptions<PolicyOptions>(nameof(PolicyOptions));

            Guard.Against.Null(options, nameof(options));

            return Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(options.Retry.RetryCount,
                    retryAttempt => TimeSpan.FromSeconds(options.Retry.SleepDuration),
                    onRetry: (response, timeSpan, retryCount, context) =>
                    {
                        if (response?.Exception != null)
                        {
                            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                            var logger = loggerFactory.CreateLogger(Configs.POLLY_GRPC_CB_LOGGER);

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
