using EventPAM.Event.Venues.Features.CreatingVenue.V1;

namespace EventPAM.UnitTest.Event.Fakes;

public class FakeValidateCreateVenueCommand : AutoFaker<CreateVenue>
{
    public FakeValidateCreateVenueCommand()
    {
        RuleFor(r => r.Name, _ => null!);
        RuleFor(r => r.Capacity, _ => 0);
    }
}
