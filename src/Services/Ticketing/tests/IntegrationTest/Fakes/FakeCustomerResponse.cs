using EventPAM.Ticketing.GrpcClient.Protos;

namespace EventPAM.IntegrationTest.Ticketing.Fakes;

public static class FakeCustomerResponse
{
    public static GetCustomerByIdResult Generate()
    {
        var result = new GetCustomerByIdResult
        {
            CustomerDto = new CustomerResponse()
            {
                Id = NewId.NextGuid().ToString(),
                Name = "Test",
                Age = 29,
                CustomerType = CustomerType.Male
            }
        };

        return result;
    }
}
