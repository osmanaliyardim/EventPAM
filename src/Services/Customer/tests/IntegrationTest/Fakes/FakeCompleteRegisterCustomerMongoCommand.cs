using EventPAM.Customer.Customers.Enums;
using EventPAM.Customer.Customers.Features.CompletingRegisterCustomer.V1;

namespace EventPAM.IntegrationTest.Customer.Fakes;

public class FakeCompleteRegisterCustomerMongoCommand : AutoFaker<CompleteRegisterCustomerMongoCommand>
{
    public FakeCompleteRegisterCustomerMongoCommand()
    {
        RuleFor(r => r.Id, _ => NewId.NextGuid());
        RuleFor(r => r.Name, _ => "customer");
        RuleFor(r => r.Age, _ => 29);
        RuleFor(r => r.CustomerType, _ => CustomerType.Male);
        RuleFor(r => r.IsDeleted, _ => false);
    }
}
