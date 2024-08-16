using EventPAM.Event.Events.Features.GettingAvailableEvents.V1;

namespace EventPAM.IntegrationTest.Event.Features;

public class GetAvailableEventsTests : EventIntegrationTestBase
{
    public GetAvailableEventsTests(TestFixture<Program, EventDbContext, EventReadDbContext> integrationTestFactory) 
        : base(integrationTestFactory)
    {

    }

    [Fact]
    public async Task ShouldReturnAvailableEvents()
    {
        // Arrange
        var command = new FakeCreateEventMongoCommand().Generate();

        await Fixture.SendAsync(command);

        var query = new GetAvailableEvents();

        // Act
        var response = (await Fixture.SendAsync(query))?.EventDtos?.Items.ToList();

        // Assert
        response?.Should().NotBeNull();
        response?.Count.Should().BeGreaterOrEqualTo(2);
    }
}
