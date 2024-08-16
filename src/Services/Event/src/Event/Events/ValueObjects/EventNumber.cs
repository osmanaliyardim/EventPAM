namespace EventPAM.Event.Events.ValueObjects;

public record EventNumber
{
    public string Value { get; }

    private EventNumber(string value)
    {
        Value = value;
    }

    public static EventNumber Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidEventNumberException(value);
        }

        return new EventNumber(value);
    }

    public static implicit operator string(EventNumber eventNumber)
    {
        return eventNumber.Value;
    }
}
