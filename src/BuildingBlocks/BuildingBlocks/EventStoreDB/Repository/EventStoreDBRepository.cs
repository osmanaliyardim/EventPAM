﻿using EventPAM.BuildingBlocks.EventStoreDB.Events;
using EventPAM.BuildingBlocks.EventStoreDB.Serialization;
using EventStore.Client;

namespace EventPAM.BuildingBlocks.EventStoreDB.Repository;

public interface IEventStoreDBRepository<T> where T : class, IAggregateEventSourcing<Guid>
{
    Task<T?> Find(Guid id, CancellationToken cancellationToken);

    Task<ulong> Add(T aggregate, CancellationToken cancellationToken);

    Task<ulong> Update(T aggregate, long? expectedRevision = null,
        CancellationToken cancellationToken = default);

    Task<ulong> Delete(T aggregate, long? expectedRevision = null, CancellationToken cancellationToken = default);
}

public class EventStoreDBRepository<T> : IEventStoreDBRepository<T> where T : class, IAggregateEventSourcing<Guid>
{
    private static readonly long _currentUserId;
    private readonly EventStoreClient eventStore;

    public EventStoreDBRepository(EventStoreClient eventStore)
    {
        this.eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
    }

    public Task<T?> Find(Guid id, CancellationToken cancellationToken)
    {
        return eventStore.AggregateStream<T>(
            id,
            cancellationToken
        );
    }

    public async Task<ulong> Add(T aggregate, CancellationToken cancellationToken = default)
    {
        var result = await eventStore.AppendToStreamAsync(
            StreamNameMapper.ToStreamId<T>(aggregate.Id),
            StreamState.NoStream,
            GetEventsToStore(aggregate),
            cancellationToken: cancellationToken
        );
        return result.NextExpectedStreamRevision;
    }

    public async Task<ulong> Update(T aggregate, long? expectedRevision = null,
        CancellationToken cancellationToken = default)
    {
        var nextVersion = expectedRevision ?? aggregate.Version;

        var result = await eventStore.AppendToStreamAsync(
            StreamNameMapper.ToStreamId<T>(aggregate.Id),
            (ulong)nextVersion,
            GetEventsToStore(aggregate),
            cancellationToken: cancellationToken
        );
        return result.NextExpectedStreamRevision;
    }

    public Task<ulong> Delete(T aggregate, long? expectedRevision = null,
        CancellationToken cancellationToken = default)
    {
        return Update(aggregate, expectedRevision, cancellationToken);
    }

    private static IEnumerable<EventData> GetEventsToStore(T aggregate)
    {
        var events = aggregate.ClearDomainEvents();

        return events
            .Select(EventStoreDBSerializer.ToJsonEventData);
    }
}
