using EventPAM.Identity.Identity.Features.GettingUserById.V1;

namespace EventPAM.IntegrationTest.Identity.Fakes;

public class FakeGetUserByIdQuery : AutoFaker<GetUserByIdQuery>
{
    public FakeGetUserByIdQuery()
    {
        RuleFor(r => r.Id, x => new Guid());
    }
}
