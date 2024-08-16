using EventPAM.Ticketing.GrpcClient.Protos;
using Google.Protobuf.WellKnownTypes;

namespace EventPAM.IntegrationTest.Ticketing.Fakes;

public static class FakeEventResponse
{
    public static GetEventByIdResult Generate()
    {
        var eventMock = new GetEventByIdResult
        {
            EventDto = new EventResponse
            {
                Id = new Guid("3c5c0000-97c6-fc34-2eb9-08db322230c9").ToString(),
                Price = 100,
                Status = EventStatus.Completed,
                VenueId = new Guid("3c5c0000-97c6-fc34-fcd3-08db322230c8").ToString(),
                EventDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc).ToTimestamp(),
                EventNumber = "1500B",
            }
        };

        return eventMock;
    }
}
