using EventPAM.Customer.Customers.Enums;
using EventPAM.Customer.Customers.Features.CompletingRegisterCustomer.V1;

namespace EventPAM.IntegrationTest.Customer.Fakes;

public sealed class FakeCompleteRegisterCustomerCommand : AutoFaker<CompleteRegisterCustomer>
{
    public FakeCompleteRegisterCustomerCommand(string name)
    {
        RuleFor(r => r.Id, _ => NewId.NextGuid());
        RuleFor(r => r.Name, _ => name);
        RuleFor(r => r.CustomerType, _ => CustomerType.Male);
        RuleFor(r => r.Age, _ => 29);
    }
}
