namespace EventPAM.Customer.Exceptions;

public class InvalidCustomerIdException : BadRequestException
{
    public InvalidCustomerIdException(Guid customerId)
        : base($"CustomerId: '{customerId}' is invalid.")
    {
    }
}
