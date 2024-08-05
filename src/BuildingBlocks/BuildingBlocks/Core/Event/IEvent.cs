using MassTransit;

namespace EventPAM.BuildingBlocks.Core.Event;

public interface IEvent : INotification
{
    Guid EventId => NewId.NextGuid();

    public DateTime OccurredOn => DateTime.Now;

    public string EventType => GetType().AssemblyQualifiedName!;
}
