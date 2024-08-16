using EventStore.Client;
using EventPAM.BuildingBlocks.EventStoreDB.Serialization;

namespace EventPAM.BuildingBlocks.EventStoreDB.Events;

public static class StreamEventExtensions
{
    public static StreamEvent? ToStreamEvent(this ResolvedEvent resolvedEvent)
    {
        var eventData = resolvedEvent.Deserialize();

        if (eventData == null)
            return null;

        var metaData = new EventMetadata(resolvedEvent.Event.EventNumber.ToUInt64(), resolvedEvent.Event.Position.CommitPosition);
        var type = typeof(StreamEvent<>).MakeGenericType(eventData.GetType());

        return (StreamEvent)Activator.CreateInstance(type, eventData, metaData)!;
    }
}
