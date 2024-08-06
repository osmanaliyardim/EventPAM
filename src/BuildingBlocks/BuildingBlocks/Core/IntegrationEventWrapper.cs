using EventPAM.BuildingBlocks.Core.Event;

namespace EventPAM.BuildingBlocks.Core;

public record IntegrationEventWrapper<TDomainEventType>(TDomainEventType DomainEvent) : IIntegrationEvent
    where TDomainEventType : IDomainEvent;
