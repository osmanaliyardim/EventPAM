using EventPAM.BuildingBlocks.EventStoreDB.Events;
using EventPAM.BuildingBlocks.EventStoreDB.Projections;
using EventPAM.Ticketing.Data;
using EventPAM.Ticketing.Ticketing.Features.CreatingTicket.V1;
using EventPAM.Ticketing.Ticketing.Models;
using MassTransit;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace EventPAM.Ticketing;

public class TicketingProjection : IProjectionProcessor
{
    private readonly TicketingReadDbContext _ticketingReadDbContext;

    public TicketingProjection(TicketingReadDbContext ticketingReadDbContext)
    {
        _ticketingReadDbContext = ticketingReadDbContext;
    }

    public async Task ProcessEventAsync<T>(StreamEvent<T> streamEvent, CancellationToken cancellationToken = default)
        where T : INotification
    {
        switch (streamEvent.Data)
        {
            case TicketingCreatedDomainEvent ticketingCreatedDomainEvent:
                await Apply(ticketingCreatedDomainEvent, cancellationToken);
                break;
        }
    }

    private async Task Apply(TicketingCreatedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var reservation =
            await _ticketingReadDbContext.Tickets.AsQueryable()
                .SingleOrDefaultAsync(t => t.Id == @event.Id && !t.IsDeleted, cancellationToken);

        if (reservation is null)
        {
            var ticketingReadModel = new TicketingReadModel
            {
                Id = NewId.NextGuid(),
                EventDetails = @event.EventDetails,
                TicketId = @event.Id,
                CustomerInfo = @event.CustomerInfo,
                IsDeleted = @event.IsDeleted
            };

            await _ticketingReadDbContext.Tickets.InsertOneAsync(ticketingReadModel, cancellationToken: cancellationToken);
        }
    }
}
