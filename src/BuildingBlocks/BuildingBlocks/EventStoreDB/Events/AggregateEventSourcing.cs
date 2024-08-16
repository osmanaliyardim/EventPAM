﻿using EventPAM.BuildingBlocks.Core.Event;
using EventPAM.BuildingBlocks.Core.Model;

namespace EventPAM.BuildingBlocks.EventStoreDB.Events;

public abstract record AggregateEventSourcing<TId> : Entity<TId>, IAggregateEventSourcing<TId>
{
    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public IDomainEvent[] ClearDomainEvents()
    {
        var dequeuedEvents = _domainEvents.ToArray();

        _domainEvents.Clear();

        return dequeuedEvents;
    }

    public virtual void When(object @event) { }
}

