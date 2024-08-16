namespace EventPAM.Customer.Customers.Exceptions;

public class CustomerNotFoundException: NotFoundException
{
    public CustomerNotFoundException(string code = default!) : base(Messages.USER_NOT_FOUND)
    {

    }
}
