using EventPAM.BuildingBlocks.Core.Event;

namespace EventPAM.BuildingBlocks.Contracts.EventBus.Messages;

public record EventCreated(Guid EventId) : IIntegrationEvent;

public record EventUpdated(Guid EventId) : IIntegrationEvent;

public record EventDeleted(Guid EventId) : IIntegrationEvent;

public record VenueCreated(Guid VenueId) : IIntegrationEvent;

public record SeatCreated(Guid SeatId) : IIntegrationEvent;

public record SeatReserved(Guid SeatId) : IIntegrationEvent;
