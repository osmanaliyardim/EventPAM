namespace EventPAM.Customer.Customers.Exceptions;

public class CustomerAlreadyExists : BadRequestException
{
    public CustomerAlreadyExists(string code = default!) : base(Messages.USER_ALREADY_EXISTS)
    {

    }
}
