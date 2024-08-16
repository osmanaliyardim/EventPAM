namespace EventPAM.IntegrationTest.Customer.Fakes;

public class FakeUserCreated : AutoFaker<UserCreated>
{
    public FakeUserCreated()
    {
        RuleFor(r => r.Id, _ => NewId.NextGuid());
        RuleFor(r => r.Name, _ => "customer");
    }
}
