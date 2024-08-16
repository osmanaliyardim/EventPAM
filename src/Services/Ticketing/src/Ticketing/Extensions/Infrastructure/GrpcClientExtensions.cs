using EventPAM.Ticketing.Configuration;
using EventPAM.Ticketing.GrpcClient.Protos;
using Microsoft.Extensions.DependencyInjection;
using EventPAM.BuildingBlocks.Polly;

namespace EventPAM.Ticketing.Extensions.Infrastructure;

public static class GrpcClientExtensions
{
    public static IServiceCollection AddGrpcClients(this IServiceCollection services)
    {
        var grpcOptions = services.GetOptions<GrpcOptions>("Grpc");

        services.AddGrpcClient<EventGrpcService.EventGrpcServiceClient>(o =>
        {
            o.Address = new Uri(grpcOptions.EventAddress);
        })
        .AddGrpcRetryPolicyHandler()
        .AddGrpcCircuitBreakerPolicyHandler();

        services.AddGrpcClient<CustomerGrpcService.CustomerGrpcServiceClient>(o =>
        {
            o.Address = new Uri(grpcOptions.CustomerAddress);
        })
        .AddGrpcRetryPolicyHandler()
        .AddGrpcCircuitBreakerPolicyHandler();;

        return services;
    }
}
