using EventPAM.Event.Events.Features.UpdatingEvent.V1;
using EventPAM.UnitTest.Event.Common;
using EventPAM.UnitTest.Event.Fakes;

namespace EventPAM.UnitTest.Event.Features.Domains;

[Collection(nameof(UnitTestFixture))]
public class UpdateEventTests
{
    [Fact]
    public void ShouldBeAbleToUpdateValidEvent()
    {
        // Arrange
        var fakeEvent = FakeEventCreate.Generate();

        // Act
        FakeEventUpdate.Generate(fakeEvent);

        // Assert
        fakeEvent.Price.Value.Should().Be(1000);
    }

    [Fact]
    public void EnqueueDomainEventOnUpdate()
    {
        // Arrange
        var fakeEvent = FakeEventCreate.Generate();
        fakeEvent.ClearDomainEvents();

        // Act
        FakeEventUpdate.Generate(fakeEvent);

        // Assert
        fakeEvent.DomainEvents.Count.Should().Be(1);
        fakeEvent.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(EventUpdatedDomainEvent));
    }
}
