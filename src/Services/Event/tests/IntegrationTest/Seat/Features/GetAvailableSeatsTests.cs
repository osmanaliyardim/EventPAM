using EventPAM.Event.GrpcServer.Protos;

namespace EventPAM.IntegrationTest.Seat.Features;

public class GetAvailableSeatsTests : EventIntegrationTestBase
{
    public GetAvailableSeatsTests(TestFixture<Program, EventDbContext, EventReadDbContext> integrationTestFactory) 
        : base(integrationTestFactory)
    {
    }

    [Fact]
    public async Task ShouldReturnAvailableSeatsFromGrpcService()
    {
        // Arrange
        var eventCommand = new FakeCreateEventMongoCommand().Generate();

        await Fixture.SendAsync(eventCommand);

        var seatCommand = new FakeCreateSeatMongoCommand(eventCommand.EventId).Generate();

        await Fixture.SendAsync(seatCommand);

        var eventGrpcClient = new EventGrpcService.EventGrpcServiceClient(Fixture.Channel);

        // Act
        var response = await eventGrpcClient.GetAvailableSeatsAsync(new GetAvailableSeatsRequest{EventId = eventCommand.EventId.ToString()});

        // Assert
        response?.Should().NotBeNull();
        response?.SeatDtos?.Count.Should().BeGreaterOrEqualTo(1);
    }
}
