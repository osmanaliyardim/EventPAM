using Ardalis.GuardClauses;
using EventPAM.BuildingBlocks.Contracts.EventBus.Messages;
using EventPAM.BuildingBlocks.Core;
using EventPAM.BuildingBlocks.Core.Event;
using EventPAM.BuildingBlocks.Web;
using EventPAM.Customer.Customers.ValueObjects;
using EventPAM.Customer.Data;
using Humanizer;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventPAM.Customer.Identity.Consumers.Registering.V1;

public class RegisterConsumerHandler : IConsumer<UserCreated>
{
    private readonly CustomerDbContext _customerDbContext;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly ILogger<RegisterConsumerHandler> _logger;
    private readonly AppOptions _options;

    public RegisterConsumerHandler(
        CustomerDbContext customerDbContext,
        IEventDispatcher eventDispatcher,
        ILogger<RegisterConsumerHandler> logger,
        IOptions<AppOptions> options)
    {
        _customerDbContext = customerDbContext;
        _eventDispatcher = eventDispatcher;
        _logger = logger;
        _options = options.Value;
    }

    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        Guard.Against.Null(context.Message, nameof(UserCreated));

        _logger.LogInformation($"Consumer for {nameof(UserCreated).Underscore()} in {_options.Name}");

        var customerExists =
            await _customerDbContext.Customer.AnyAsync(x => x.Name.Value == context.Message.Name);

        if (customerExists)
            return;

        var customer = Customers.Models.Customer.Create
            (CustomerId.Of(NewId.NextGuid()), context.Message.Name);

        await _customerDbContext.AddAsync(customer);
        await _customerDbContext.SaveChangesAsync();

        await _eventDispatcher.SendAsync(
            new CustomerCreatedDomainEvent(customer.Id, customer.Name),
            typeof(IInternalCommand)
        );
    }
}
