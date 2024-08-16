using EventPAM.BuildingBlocks.Behaviors.Authorization;
using EventPAM.BuildingBlocks.Behaviors.Caching;
using EventPAM.BuildingBlocks.Behaviors.Logging;
using EventPAM.BuildingBlocks.Behaviors.Transaction;
using EventPAM.BuildingBlocks.Behaviors.Validation;
using EventPAM.BuildingBlocks.EFCore;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace EventPAM.Customer.Extensions.Infrastructure;

public static class MediatRExtensions
{
    public static IServiceCollection AddCustomMediatR(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(CustomerRoot).Assembly);
            configuration.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
            configuration.AddOpenBehavior(typeof(CachingBehavior<,>));
            configuration.AddOpenBehavior(typeof(CacheRemovingBehavior<,>));
            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
            configuration.AddOpenBehavior(typeof(RequestValidationBehavior<,>));
            configuration.AddOpenBehavior(typeof(TransactionScopeBehavior<,>));
            
        });

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(EfTxBehavior<,>));

        return services;
    }
}
