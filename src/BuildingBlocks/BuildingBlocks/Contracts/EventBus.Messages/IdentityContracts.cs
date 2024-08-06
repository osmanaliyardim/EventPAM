using EventPAM.BuildingBlocks.Core.Event;

namespace EventPAM.BuildingBlocks.Contracts.EventBus.Messages;

public record UserCreated(Guid Id, string Name) : IIntegrationEvent;
