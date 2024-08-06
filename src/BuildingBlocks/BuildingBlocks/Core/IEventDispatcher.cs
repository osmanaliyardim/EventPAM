using EventPAM.BuildingBlocks.Core.Event;

namespace EventPAM.BuildingBlocks.Core;

public interface IEventDispatcher
{
    public Task SendAsync<T>
        (IReadOnlyList<T> events, Type type = null!, CancellationToken cancellationToken = default)
        where T : IEvent;

    public Task SendAsync<T>
        (T @event, Type type = null!, CancellationToken cancellationToken = default)
        where T : IEvent;
}
