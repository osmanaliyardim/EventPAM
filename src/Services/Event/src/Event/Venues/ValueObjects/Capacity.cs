using EventPAM.Event.Venues.Exceptions;

namespace EventPAM.Event.Venues.ValueObjects;

public record Capacity
{
    public int Value { get; }
    
    private Capacity(int value)
    {
        Value = value;
    }

    public static Capacity Of(int value)
    {
        if (value < 1)
        {
            throw new InvalidCapacityException();
        }

        return new Capacity(value);
    }

    public static implicit operator int(Capacity capacity)
    {
        return capacity.Value;
    }
}
