using EventPAM.BuildingBlocks.CrossCuttingConcerns.Exceptions.Types;

namespace EventPAM.Ticketing.Ticketing.Exceptions;

public class InvalidCustomerNameException : BadRequestException
{
    public InvalidCustomerNameException(string customerName)
        : base($"Customer Name: '{customerName}' is invalid.")
    {

    }
}
