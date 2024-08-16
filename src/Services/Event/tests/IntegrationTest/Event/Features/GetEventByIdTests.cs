using EventPAM.Event.Events.Features.GettingEventById.V1;
using EventPAM.Event.GrpcServer.Protos;

namespace EventPAM.IntegrationTest.Event.Features;

public class GetEventByIdTests : EventIntegrationTestBase
{
    public GetEventByIdTests(TestFixture<Program, EventDbContext, EventReadDbContext> integrationTestFactory) 
        : base(integrationTestFactory)
    {

    }

    [Fact]
    public async Task ShouldRetriveAnEventByIdCorrectly()
    {
        //Arrange
        var command = new FakeCreateEventMongoCommand().Generate();

        await Fixture.SendAsync(command);

        var query = new GetEventById(command.EventId);

        // Act
        var response = await Fixture.SendAsync(query);

        // Assert
        response.Should().NotBeNull();
        response?.EventDto?.EventId.Should().Be(command.EventId);
    }

    [Fact]
    public async Task ShouldRetriveAnEventByIdFromGrpcService()
    {
        //Arrange
        var command = new FakeCreateEventMongoCommand().Generate();

        await Fixture.SendAsync(command);

        var eventGrpcClient = new EventGrpcService.EventGrpcServiceClient(Fixture.Channel);

        // Act
        var response = await eventGrpcClient.GetByIdAsync(new GetByIdRequest {Id = command.EventId.ToString()}).ResponseAsync;

        // Assert
        response?.Should().NotBeNull();
        response?.EventDto.Id.Should().Be(command.EventId.ToString());
    }
}
