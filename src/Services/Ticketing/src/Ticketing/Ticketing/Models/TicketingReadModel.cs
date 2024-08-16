using EventPAM.BuildingBlocks.Core.Model;
using EventPAM.Ticketing.Ticketing.ValueObjects;

namespace EventPAM.Ticketing.Ticketing.Models;

public record TicketingReadModel : Aggregate<Guid>
{
    public required Guid TicketId { get; init; }

    public required EventDetails EventDetails { get; init; }

    public required CustomerInfo CustomerInfo { get; init; }
}
