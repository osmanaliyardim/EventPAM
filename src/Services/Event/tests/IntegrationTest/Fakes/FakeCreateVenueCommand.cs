using EventPAM.Event.Venues.Features.CreatingVenue.V1;
using MassTransit;

namespace EventPAM.IntegrationTest.Event.Fakes;

public class FakeCreateVenueCommand : AutoFaker<CreateVenue>
{
    public FakeCreateVenueCommand()
    {
        RuleFor(r => r.Id, _ => NewId.NextGuid());
        RuleFor(r => r.Capacity, _ => 150000);
    }
}
