namespace EventPAM.Customer.Customers.Exceptions;

public class InvalidNameException : BadRequestException
{
    public InvalidNameException() : base(Messages.NAME_INVALID)
    {

    }
}
