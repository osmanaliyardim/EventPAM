using EventPAM.Event.Events.Features.CreatingEvent.V1;
using EventPAM.UnitTest.Event.Common;
using EventPAM.UnitTest.Event.Fakes;

namespace EventPAM.UnitTest.Event.Features.Domains;

[Collection(nameof(UnitTestFixture))]
public class CreateEventTests
{
    [Fact]
    public void ShouldBeAbleToCreateValidEvent()
    {
        // Arrange + Act
        var fakeEvent = FakeEventCreate.Generate();

        // Assert
        fakeEvent.Should().NotBeNull();
    }

    [Fact]
    public void EnqueueDomainEventOnCreate()
    {
        // Arrange + Act
        var fakeEvent = FakeEventCreate.Generate();

        // Assert
        fakeEvent.DomainEvents.Count.Should().Be(1);
        fakeEvent.DomainEvents.FirstOrDefault().Should().BeOfType(typeof(EventCreatedDomainEvent));
    }
}
