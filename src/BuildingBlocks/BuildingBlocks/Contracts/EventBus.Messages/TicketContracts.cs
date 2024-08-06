using EventPAM.BuildingBlocks.Core.Event;

namespace EventPAM.BuildingBlocks.Contracts.EventBus.Messages;

public record TicketCreated(Guid Id) : IIntegrationEvent;
