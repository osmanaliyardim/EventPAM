using EventPAM.BuildingBlocks.Core.Event;

namespace EventPAM.BuildingBlocks.Core.Model;

public interface IAggregate<T> : IAggregate, IEntity<T>
{
}

public interface IAggregate : IEntity
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }

    IEvent[] ClearDomainEvents();
}
