using EventPAM.BuildingBlocks.Behaviors.Authorization;
using EventPAM.BuildingBlocks.Behaviors.Logging;
using EventPAM.BuildingBlocks.Behaviors.Transaction;
using EventPAM.BuildingBlocks.Behaviors.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace EventPAM.Ticketing.Extensions.Infrastructure;

public static class MediatRExtensions
{
    public static IServiceCollection AddCustomMediatR(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(TicketingRoot).Assembly);
            configuration.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
            configuration.AddOpenBehavior(typeof(RequestValidationBehavior<,>));
            configuration.AddOpenBehavior(typeof(TransactionScopeBehavior<,>));
        });

        return services;
    }
}
