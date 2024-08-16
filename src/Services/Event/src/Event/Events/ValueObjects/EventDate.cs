namespace EventPAM.Event.Events.ValueObjects;

public record EventDate
{
    public DateTime Value { get; }

    private EventDate(DateTime value)
    {
        Value = value;
    }

    public static EventDate Of(DateTime value)
    {
        if (value == default)
        {
            throw new InvalidEventDateException(value);
        }

        return new EventDate(value);
    }

    public static implicit operator DateTime(EventDate eventDate)
    {
        return eventDate.Value;
    }
}
