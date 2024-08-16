using EventPAM.BuildingBlocks.Core.Event;
using EventPAM.BuildingBlocks.Core.Model;

namespace EventPAM.BuildingBlocks.EventStoreDB.Events;

public interface IAggregateEventSourcing : IProjection, IEntity
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    IDomainEvent[] ClearDomainEvents();
}

public interface IAggregateEventSourcing<T> : IAggregateEventSourcing, IEntity<T>
{

}
