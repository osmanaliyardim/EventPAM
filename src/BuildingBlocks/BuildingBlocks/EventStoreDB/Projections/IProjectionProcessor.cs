using EventPAM.BuildingBlocks.EventStoreDB.Events;

namespace EventPAM.BuildingBlocks.EventStoreDB.Projections;

public interface IProjectionProcessor
{
    Task ProcessEventAsync<T>(StreamEvent<T> streamEvent, CancellationToken cancellationToken = default)
        where T : INotification;
}
