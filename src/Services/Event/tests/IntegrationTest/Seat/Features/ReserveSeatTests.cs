using EventPAM.Event.GrpcServer.Protos;

namespace EventPAM.IntegrationTest.Seat.Features;

public class ReserveSeatTests : EventIntegrationTestBase
{
    public ReserveSeatTests(TestFixture<Program, EventDbContext, EventReadDbContext> integrationTestFactory) 
        : base(integrationTestFactory)
    {

    }

    [Fact]
    public async Task ShouldReturnValidReserveSeatFromGrpcService()
    {
        // Arrange
        var eventCommand = new FakeCreateEventCommand().Generate();

        await Fixture.SendAsync(eventCommand);

        var seatCommand = new FakeCreateSeatCommand(eventCommand.Id).Generate();

        await Fixture.SendAsync(seatCommand);

        var eventGrpcClient = new EventGrpcService.EventGrpcServiceClient(Fixture.Channel);

        // Act
        var response = await eventGrpcClient.ReserveSeatAsync(new ReserveSeatRequest()
        {
            EventId = seatCommand.EventId.ToString(),
            SeatNumber = seatCommand.SeatNumber
        });

        // Assert
        response?.Should().NotBeNull();
        response?.Id.Should().Be(seatCommand?.Id.ToString());
    }
}
