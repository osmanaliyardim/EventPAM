namespace EventPAM.EndToEndTest.Event.Features;

public class CreateEventTests : EventEndToEndTestBase
{
    public CreateEventTests(TestFixture<Program, EventDbContext, EventReadDbContext> integrationTestFixture) 
        : base(integrationTestFixture)
    {

    }

    [Fact]
    public async Task ShouldCreateNewEventToDbAndPublishMessageToBroker()
    {
        //Arrange
        var command = new FakeCreateEventCommand().Generate();

        // Act
        var route = ApiRoutes.Event.CreateEvent;
        var result = await Fixture.HttpClient.PostAsJsonAsync(route, command);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
