using EventPAM.BuildingBlocks.Contracts.EventBus.Messages;
using EventPAM.BuildingBlocks.Core;
using EventPAM.BuildingBlocks.Core.Event;
using EventPAM.Customer.Customers.Enums;
using EventPAM.Customer.Customers.Features.CompletingRegisterCustomer.V1;
using EventPAM.Customer.Identity.Consumers.Registering.V1;

namespace EventPAM.Customer;

public sealed class EventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent @event)
    {
        return @event switch
        {
            CustomerRegistrationCompletedDomainEvent e => new CustomerRegistrationCompleted(e.Id),
            CustomerCreatedDomainEvent e => new CustomerCreated(e.Id),
            _ => null
        };
    }

    public IInternalCommand? MapToInternalCommand(IDomainEvent @event)
    {
        return @event switch
        {
            CustomerRegistrationCompletedDomainEvent e => new CompleteRegisterCustomerMongoCommand
                (e.Id, e.Name, e.CustomerType, e.Age, e.IsDeleted),
            CustomerCreatedDomainEvent e => new CompleteRegisterCustomerMongoCommand
                (e.Id, e.Name, CustomerType.Unknown, 0, e.IsDeleted),
            _ => null
        };
    }
}
