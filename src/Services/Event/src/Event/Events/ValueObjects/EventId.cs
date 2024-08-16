namespace EventPAM.Event.Events.ValueObjects;

public record EventId
{
    public Guid Value { get; }

    private EventId(Guid value)
    {
        Value = value;
    }

    public static EventId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidEventIdException(value);
        }

        return new EventId(value);
    }

    public static implicit operator Guid(EventId eventId)
    {
        return eventId.Value;
    }
}
