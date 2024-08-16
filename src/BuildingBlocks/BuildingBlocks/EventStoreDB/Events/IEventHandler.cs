using EventPAM.BuildingBlocks.Core.Event;

namespace EventPAM.BuildingBlocks.EventStoreDB.Events;

public interface IEventHandler<in TEvent>: INotificationHandler<TEvent>
    where TEvent : IEvent
{

}
