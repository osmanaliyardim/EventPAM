using EventPAM.Ticketing.GrpcClient.Protos;
using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;

namespace EventPAM.IntegrationTest.Ticketing.Features;

public class CreateTicketingTests : TicketingIntegrationTestBase
{
    public CreateTicketingTests(TestReadFixture<Program, TicketingReadDbContext> integrationTestFixture) : base(
        integrationTestFixture)
    {

    }

    protected override void RegisterTestsServices(IServiceCollection services)
    {
        MockEventGrpcServices(services);
        MockCustomerGrpcServices(services);
    }

    [Fact]
    public async Task ShouldCreateTicketingToEventStoreCorrectly()
    {
        // Arrange
        var command = new FakeCreateTicketingCommand().Generate();

        // Act
        var response = await Fixture.SendAsync(command);

        // Assert
        response?.Id.Should().BeGreaterOrEqualTo(0);

        (await Fixture.WaitForPublishing<TicketingCreated>()).Should().Be(true);
    }

    private void MockCustomerGrpcServices(IServiceCollection services)
    {
        services.Replace(ServiceDescriptor.Singleton(x =>
        {
            var mockCustomer = Substitute.For<CustomerGrpcService.CustomerGrpcServiceClient>();

            mockCustomer.GetByIdAsync(Arg.Any<GetCustomerByIdRequest>())
                .Returns(TestCalls.AsyncUnaryCall(Task.FromResult(FakeCustomerResponse.Generate()),
                    Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => new Metadata(), () => { }));

            return mockCustomer;
        }));
    }

    private void MockEventGrpcServices(IServiceCollection services)
    {
        services.Replace(ServiceDescriptor.Singleton(x =>
        {
            var mockEvent = Substitute.For<EventGrpcService.EventGrpcServiceClient>();

            mockEvent.GetByIdAsync(Arg.Any<GetByIdRequest>())
                .Returns(TestCalls.AsyncUnaryCall(Task.FromResult(FakeEventResponse.Generate()),
                    Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => new Metadata(), () => { }));

            mockEvent.GetAvailableSeatsAsync(Arg.Any<GetAvailableSeatsRequest>())
                .Returns(TestCalls.AsyncUnaryCall(Task.FromResult(FakeGetAvailableSeatsResponse.Generate()),
                    Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => new Metadata(), () => { }));

            mockEvent.ReserveSeatAsync(Arg.Any<ReserveSeatRequest>())
                .Returns(TestCalls.AsyncUnaryCall(Task.FromResult(FakeReserveSeatResponse.Generate()),
                    Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => new Metadata(), () => { }));

            return mockEvent;
        }));
    }
}
