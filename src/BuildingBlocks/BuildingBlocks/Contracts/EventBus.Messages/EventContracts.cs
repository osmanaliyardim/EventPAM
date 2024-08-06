using EventPAM.BuildingBlocks.Core.Event;

namespace EventPAM.BuildingBlocks.Contracts.EventBus.Messages;

public record EventCreated(Guid Id) : IIntegrationEvent;

public record EventUpdated(Guid Id) : IIntegrationEvent;

public record EventDeleted(Guid Id) : IIntegrationEvent;

public record VenueCreated(Guid Id) : IIntegrationEvent;

public record VenueUpdated(Guid Id) : IIntegrationEvent;

public record SeatCreated(Guid Id) : IIntegrationEvent;

public record SeatReserved(Guid Id) : IIntegrationEvent;
