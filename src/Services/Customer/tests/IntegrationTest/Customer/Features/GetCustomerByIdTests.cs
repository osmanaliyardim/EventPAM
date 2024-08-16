using EventPAM.Customer.Customers.Features.GettingCustomerById.V1;
using EventPAM.Customer.GrpcServer.Protos;

namespace EventPAM.IntegrationTest.Customer.Features;

public class GetCustomerByIdTests : CustomerIntegrationTestBase
{
    public GetCustomerByIdTests(TestFixture<Program, CustomerDbContext, CustomerReadDbContext> integrationTestFactory) 
        : base(integrationTestFactory)
    {

    }

    [Fact]
    public async Task ShouldRetriveACustomerByIdCorrectly()
    {
        // Arrange
        var command = new FakeCompleteRegisterCustomerMongoCommand().Generate();

        await Fixture.SendAsync(command);

        var query = new GetCustomerById(command.Id);

        // Act
        var response = await Fixture.SendAsync(query);

        // Assert
        response.Should().NotBeNull();
        response?.CustomerDto?.Id.Should().Be(command.Id);
    }

    [Fact]
    public async Task ShouldRetriveACustomerByIdFromGrpcService()
    {
        // Arrange
        var command = new FakeCompleteRegisterCustomerMongoCommand().Generate();

        await Fixture.SendAsync(command);

        var customerGrpcClient = new CustomerGrpcService.CustomerGrpcServiceClient(Fixture.Channel);

        // Act
        var response = await customerGrpcClient.GetByIdAsync(new GetByIdRequest { Id = command.Id.ToString() });

        // Assert
        response?.Should().NotBeNull();
        response?.CustomerDto?.Id.Should().Be(command.Id.ToString());
    }
}
