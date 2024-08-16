using EventPAM.BuildingBlocks.Contracts.EventBus.Messages;

namespace EventPAM.IntegrationTest.Venue.Features;

public class CreateVenueTests : EventIntegrationTestBase
{
    public CreateVenueTests(TestFixture<Program, EventDbContext, EventReadDbContext> integrationTestFactory) 
        : base(integrationTestFactory)
    {

    }

    [Fact]
    public async Task ShouldCreateNewVenueToDbAndPublishMessageToBroker()
    {
        // Arrange
        var command = new FakeCreateVenueCommand().Generate();

        // Act
        var response = await Fixture.SendAsync(command);

        // Assert
        response?.VenueId.Should().Be(command.Id);

        (await Fixture.WaitForPublishing<VenueCreated>()).Should().Be(true);
    }
}
