namespace EventPAM.Customer.Customers.Exceptions;

public class InvalidAgeException : BadRequestException
{
    public InvalidAgeException() : base(Messages.AGE_INVALID)
    {

    }
}
