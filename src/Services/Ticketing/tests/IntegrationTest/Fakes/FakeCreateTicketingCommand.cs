using EventPAM.Ticketing.Ticketing.Features.CreatingTicket.V1;

namespace EventPAM.IntegrationTest.Ticketing.Fakes;

public sealed class FakeCreateTicketingCommand : AutoFaker<CreateTicketing>
{
    public FakeCreateTicketingCommand()
    {
        RuleFor(r => r.Id, _ => NewId.NextGuid());
        RuleFor(r => r.EventId, _ => new Guid("3c5c0000-97c6-fc34-2eb9-08db322230c9"));
        RuleFor(r => r.CustomerId, _ => new Guid("4c5c8888-97c6-fc34-2eb9-18db322230c1"));
    }
}
