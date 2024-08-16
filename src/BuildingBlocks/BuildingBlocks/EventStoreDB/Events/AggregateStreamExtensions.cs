﻿using EventStore.Client;
using EventPAM.BuildingBlocks.EventStoreDB.Serialization;

namespace EventPAM.BuildingBlocks.EventStoreDB.Events;

public static class AggregateStreamExtensions
{
    public static async Task<T?> AggregateStream<T>(
        this EventStoreClient eventStore,
        Guid id,
        CancellationToken cancellationToken,
        ulong? fromVersion = null
    ) where T : class, IProjection
    {
        var readResult = eventStore.ReadStreamAsync(
            Direction.Forwards,
            StreamNameMapper.ToStreamId<T>(id),
            fromVersion ?? StreamPosition.Start,
            cancellationToken: cancellationToken
        );

        // ToDo: consider adding extension method for the aggregation and deserialization
        var aggregate = (T)Activator.CreateInstance(typeof(T), true)!;

        if (await readResult.ReadState == ReadState.StreamNotFound)
        {
            return null;
        }

        await foreach (var @event in readResult)
        {
            var eventData = @event.Deserialize();

            aggregate.When(eventData!);
        }

        return aggregate;
    }
}
