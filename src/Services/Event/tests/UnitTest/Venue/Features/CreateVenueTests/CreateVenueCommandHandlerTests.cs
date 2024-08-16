using EventPAM.Event.Venues.Features.CreatingVenue.V1;
using EventPAM.UnitTest.Event.Common;
using EventPAM.UnitTest.Event.Fakes;

namespace EventPAM.UnitTest.Venue.Features.CreateVenueTests;

[Collection(nameof(UnitTestFixture))]
public class CreateVenueCommandHandlerTests
{
    private readonly UnitTestFixture _fixture;
    private readonly CreateVenueHandler _handler;

    public Task<CreateVenueResult> Act(CreateVenue command, CancellationToken cancellationToken) =>
        _handler.Handle(command, cancellationToken);

    public CreateVenueCommandHandlerTests(UnitTestFixture fixture)
    {
        _fixture = fixture;
        _handler = new CreateVenueHandler(_fixture.DbContext);
    }

    [Fact]
    public async Task HandlerWithValidCommandShouldCreateNewVenueAndReturnCorrectVenueDto()
    {
        // Arrange
        var command = new FakeCreateVenueCommand().Generate();

        // Act
        var response = await Act(command, CancellationToken.None);

        // Assert
        var entity = await _fixture.DbContext.Venues.FindAsync(response?.VenueId);

        entity?.Should().NotBeNull();
        response?.VenueId.Should().Be(entity!.Id);
    }

    [Fact]
    public async Task HandlerWithNullCommandShouldThrowArgumentException()
    {
        // Arrange
        CreateVenue command = null!;

        // Act
        Func<Task> act = async () => { await Act(command, CancellationToken.None); };

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
