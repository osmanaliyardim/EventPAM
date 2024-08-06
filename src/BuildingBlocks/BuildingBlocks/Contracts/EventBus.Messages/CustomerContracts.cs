using EventPAM.BuildingBlocks.Core.Event;

namespace EventPAM.BuildingBlocks.Contracts.EventBus.Messages;

public record CustomerRegistrationCompleted(Guid Id) : IIntegrationEvent;

public record CustomerCreated(Guid Id) : IIntegrationEvent;
