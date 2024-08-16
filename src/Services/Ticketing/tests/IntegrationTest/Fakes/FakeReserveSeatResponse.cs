using EventPAM.Ticketing.GrpcClient.Protos;

namespace EventPAM.IntegrationTest.Ticketing.Fakes;

public static class FakeReserveSeatResponse
{
    public static ReserveSeatResult Generate()
    {
        var result = new ReserveSeatResult();
        result.Id = NewId.NextGuid().ToString();

        return result;
    }
}
