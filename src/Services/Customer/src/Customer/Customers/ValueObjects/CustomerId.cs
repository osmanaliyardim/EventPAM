using EventPAM.Customer.Exceptions;

namespace EventPAM.Customer.Customers.ValueObjects;

public record CustomerId
{
    public Guid Value { get; }

    private CustomerId(Guid value)
    {
        Value = value;
    }

    public static CustomerId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidCustomerIdException(value);
        }

        return new CustomerId(value);
    }

    public static implicit operator Guid(CustomerId customerId)
    {
        return customerId.Value;
    }
}
