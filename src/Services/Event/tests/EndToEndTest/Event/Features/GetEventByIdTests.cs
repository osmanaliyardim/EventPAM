namespace EventPAM.EndToEndTest.Event.Features;

public class GetEventByIdTests: EventEndToEndTestBase
{
    public GetEventByIdTests(TestFixture<Program, EventDbContext, EventReadDbContext> integrationTestFixture) 
        : base(integrationTestFixture)
    {

    }

    [Fact]
    public async Task ShouldRetriveAnEventByIdCorrectly()
    {
        //Arrange
        var command = new FakeCreateEventMongoCommand().Generate();

        await Fixture.SendAsync(command);

        // Act
        var route = ApiRoutes.Event.GetEventById.Replace(ApiRoutes.Event.Id, command.EventId.ToString());
        var result = await Fixture.HttpClient.GetAsync(route);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
