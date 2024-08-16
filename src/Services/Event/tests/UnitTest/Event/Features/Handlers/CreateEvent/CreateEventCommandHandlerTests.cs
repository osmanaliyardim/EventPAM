using EventPAM.Event.Events.ValueObjects;
using EventPAM.UnitTest.Event.Common;
using EventPAM.UnitTest.Event.Fakes;

namespace EventPAM.UnitTest.Event.Features.Handlers.CreateEvent;

using EventPAM.Event.Events.Features.CreatingEvent.V1;

[Collection(nameof(UnitTestFixture))]
public class CreateEventCommandHandlerTests
{
    private readonly UnitTestFixture _fixture;
    private readonly CreateEventHandler _handler;

    public Task<CreateEventResult> Act(CreateEvent command, CancellationToken cancellationToken)
    {
        return _handler.Handle(command, cancellationToken);
    }

    public CreateEventCommandHandlerTests(UnitTestFixture fixture)
    {
        _fixture = fixture;
        _handler = new CreateEventHandler(fixture.DbContext);
    }

    [Fact]
    public async Task HandlerWithValidCommandShouldCreateNewEventAndReturnCorrectEventDto()
    {
        // Arrange
        var command = new FakeCreateEventCommand().Generate();

        // Act
        var response = await Act(command, CancellationToken.None);

        // Assert
        var entity = await _fixture.DbContext.Events.FindAsync(EventId.Of(response.EventId));

        entity?.Should().NotBeNull();
        response?.EventId.Should().Be(entity!.Id);
    }

    [Fact]
    public async Task HandlerWithNullCommandShouldThrowArgumentException()
    {
        // Arrange
        CreateEvent command = null!;

        // Act
        Func<Task> act = async () => { await Act(command!, CancellationToken.None); };

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
