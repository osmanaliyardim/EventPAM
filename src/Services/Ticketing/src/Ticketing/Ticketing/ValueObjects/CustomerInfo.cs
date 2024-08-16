using EventPAM.Ticketing.Ticketing.Exceptions;

namespace EventPAM.Ticketing.Ticketing.ValueObjects;

public record CustomerInfo
{
    public string Name { get; }

    private CustomerInfo(string name)
    {
        Name = name;
    }

    public static CustomerInfo Of(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidCustomerNameException(name);
        }

        return new CustomerInfo(name);
    }
}
