namespace EventPAM.IntegrationTest.Customer.Features;

public class CompleteRegisterCustomerTests : CustomerIntegrationTestBase
{

    public CompleteRegisterCustomerTests(TestFixture<Program, CustomerDbContext, CustomerReadDbContext> integrationTestFactory) 
        : base(integrationTestFactory)
    {

    }

    [Fact]
    public async Task ShouldCompleteRegisterCustomerAndUpdateToDb()
    {
        // Arrange
        var userCreated = new FakeUserCreated().Generate();

        await Fixture.Publish(userCreated);
        (await Fixture.WaitForPublishing<UserCreated>()).Should().Be(true);
        (await Fixture.WaitForConsuming<UserCreated>()).Should().Be(true);

        var command = new FakeCompleteRegisterCustomerCommand(userCreated.Name).Generate();

        // Act
        var response = await Fixture.SendAsync(command);

        // Assert
        response.Should().NotBeNull();
        response?.CustomerDto?.Name.Should().Be(userCreated.Name);
        response?.CustomerDto?.CustomerType.ToString().Should().Be(command.CustomerType.ToString());
        response?.CustomerDto?.Age.Should().Be(command.Age);
    }
}
