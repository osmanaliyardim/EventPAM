using EventPAM.Event.Events.Features.CreatingSeat.V1;
using EventPAM.Event.Seats.ValueObjects;
using EventPAM.UnitTest.Event.Common;
using EventPAM.UnitTest.Event.Fakes;

namespace EventPAM.UnitTest.Seat.Features;

[Collection(nameof(UnitTestFixture))]
public class CreateSeatCommandHandlerTests
{
    private readonly UnitTestFixture _fixture;
    private readonly CreateSeatCommandHandler _handler;

    public CreateSeatCommandHandlerTests(UnitTestFixture fixture)
    {
        _fixture = fixture;
        _handler = new CreateSeatCommandHandler(_fixture.DbContext);
    }

    public Task<CreateSeatResult> Act(CreateSeat command, CancellationToken cancellationToken)
    {
        return _handler.Handle(command, cancellationToken);
    }

    [Fact]
    public async Task HandlerWithValidCommandShouldCreateNewSeatAndReturnCorrectSeatDto()
    {
        // Arrange
        var command = new FakeCreateSeatCommand().Generate();

        // Act
        var response = await Act(command, CancellationToken.None);

        // Assert
        var entity = await _fixture.DbContext.Seats.FindAsync(SeatId.Of(response.SeatId));

        entity?.Should().NotBeNull();
        response?.SeatId.Should().Be(entity!.Id);
    }

    [Fact]
    public async Task HandlerWithNullCommandShouldThrowArgumentException()
    {
        // Arrange
        CreateSeat command = null!;

        // Act
        var act = async () => { await Act(command!, CancellationToken.None); };

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
