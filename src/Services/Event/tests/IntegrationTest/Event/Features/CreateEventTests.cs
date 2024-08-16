using EventPAM.BuildingBlocks.Contracts.EventBus.Messages;

namespace EventPAM.IntegrationTest.Event.Features;

public class CreateEventTests : EventIntegrationTestBase
{
    public CreateEventTests(TestFixture<Program, EventDbContext, EventReadDbContext> integrationTestFactory) 
        : base(integrationTestFactory)
    {

    }

    [Fact]
    public async Task ShouldCreateNewEventToDbAndPublishMessageToBroker()
    {
        //Arrange
        var command = new FakeCreateEventCommand().Generate();

        // Act
        var response = await Fixture.SendAsync(command);

        // Assert
        response.Should().NotBeNull();
        response?.EventId.Should().Be(command.Id);

        (await Fixture.WaitForPublishing<EventCreated>()).Should().Be(true);
    }
}
