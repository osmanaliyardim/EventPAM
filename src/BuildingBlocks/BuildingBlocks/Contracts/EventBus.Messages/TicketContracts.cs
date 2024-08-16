using EventPAM.BuildingBlocks.Core.Event;

namespace EventPAM.BuildingBlocks.Contracts.EventBus.Messages;

public record TicketingCreated(Guid Id) : IIntegrationEvent;
