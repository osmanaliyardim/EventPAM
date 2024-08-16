using EventPAM.BuildingBlocks.Core.Event;

namespace EventPAM.Customer.Identity.Consumers.Registering.V1;

public record CustomerCreatedDomainEvent(Guid Id, string Name, bool IsDeleted = false) : IDomainEvent;
