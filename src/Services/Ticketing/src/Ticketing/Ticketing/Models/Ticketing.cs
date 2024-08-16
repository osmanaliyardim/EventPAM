using EventPAM.BuildingBlocks.EventStoreDB.Events;
using EventPAM.Ticketing.Ticketing.Features.CreatingTicket.V1;
using EventPAM.Ticketing.Ticketing.ValueObjects;

namespace EventPAM.Ticketing.Ticketing.Models;

public record Ticketing : AggregateEventSourcing<Guid>
{
    public EventDetails EventDetails { get; private set; } = default!;

    public CustomerInfo CustomerInfo { get; private set; } = default!;

    public static Ticketing Create(Guid id, CustomerInfo customerInfo, EventDetails eventDetails, 
        bool isDeleted = false, long? userId = null)
    {
        var ticketing = new Ticketing { Id = id, CustomerInfo = customerInfo, EventDetails = eventDetails, IsDeleted = isDeleted };

        var @event = new TicketingCreatedDomainEvent(ticketing.Id, ticketing.CustomerInfo, ticketing.EventDetails)
        {
            IsDeleted = ticketing.IsDeleted,
            CreatedAt = DateTime.Now,
            CreatedBy = userId
        };

        ticketing.AddDomainEvent(@event);
        ticketing.Apply(@event);

        return ticketing;
    }

    public override void When(object @event)
    {
        switch (@event)
        {
            case TicketingCreatedDomainEvent ticketingCreated:
            {
                Apply(ticketingCreated);
                return;
            }
        }
    }

    private void Apply(TicketingCreatedDomainEvent @event)
    {
        Id = @event.Id;
        EventDetails = @event.EventDetails;
        CustomerInfo = @event.CustomerInfo;
        IsDeleted = @event.IsDeleted;
        Version++;
    }
}
