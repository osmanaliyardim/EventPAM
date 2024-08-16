using EventPAM.BuildingBlocks.Contracts.EventBus.Messages;
using EventPAM.Event.Data.Seed;
using EventPAM.Event.Events.Features.DeletingEvent.V1;
using EventPAM.Event.Events.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace EventPAM.IntegrationTest.Event.Features;

public class DeleteEventTests : EventIntegrationTestBase
{
    public DeleteEventTests(TestFixture<Program, EventDbContext, EventReadDbContext> integrationTestFactory) 
        : base(integrationTestFactory)
    {

    }

    [Fact]
    public async Task ShouldDeleteEventFromDb()
    {
        // Arrange
        var eventEntity = await Fixture.FindAsync<EventPAM.Event.Events.Models.Event, EventId>(InitialData.Events.First().Id);
        var command = new DeleteEvent(eventEntity.Id.Value);

        // Act
        await Fixture.SendAsync(command);
        var deletedEvent = (await Fixture.ExecuteDbContextAsync(db => db.Events
                .Where(x => x.Id == EventId.Of(command.EventId))
                .IgnoreQueryFilters()
                .ToListAsync())
            ).FirstOrDefault();

        // Assert
        deletedEvent?.IsDeleted.Should().BeTrue();

        (await Fixture.WaitForPublishing<EventDeleted>()).Should().Be(true);
    }
}
