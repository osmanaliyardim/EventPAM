using EventPAM.BuildingBlocks.Contracts.EventBus.Messages;
using EventPAM.Event.Data.Seed;
using EventPAM.Event.Events.ValueObjects;

namespace EventPAM.IntegrationTest.Event.Features;

public class UpdateEventTests : EventIntegrationTestBase
{
    public UpdateEventTests(TestFixture<Program, EventDbContext, EventReadDbContext> integrationTestFactory)
        : base(integrationTestFactory)
    {

    }

    [Fact]
    public async Task ShouldUpdateEventToDbAndPublishMessageToBroker()
    {
        // Arrange
        var eventEntity = await Fixture.FindAsync<EventPAM.Event.Events.Models.Event, EventId>(InitialData.Events.First().Id);
        var command = new FakeUpdateEventCommand(eventEntity).Generate();

        // Act
        var response = await Fixture.SendAsync(command);

        // Assert
        response.Should().NotBeNull();
        response?.Id.Should().Be(eventEntity.Id);

        (await Fixture.WaitForPublishing<EventUpdated>()).Should().Be(true);
    }
}
