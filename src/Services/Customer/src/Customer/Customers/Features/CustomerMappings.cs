using EventPAM.Customer.Customers.Dtos;
using EventPAM.Customer.Customers.Features.CompletingRegisterCustomer.V1;
using EventPAM.Customer.Customers.ValueObjects;
using Mapster;
using MassTransit;

namespace EventPAM.Customer.Customers.Features;

public class CustomerMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CompleteRegisterCustomerMongoCommand, CustomerReadModel>()
            .Map(d => d.Id, s => NewId.NextGuid())
            .Map(d => d.CustomerId, s => CustomerId.Of(s.Id));

        config.NewConfig<CompleteRegisterCustomerRequestDto, CompleteRegisterCustomer>()
            .ConstructUsing(x => new CompleteRegisterCustomer(x.Name, x.CustomerType, x.Age));

        config.NewConfig<CustomerReadModel, CustomerDto>()
            .ConstructUsing(x => new CustomerDto(x.CustomerId, x.Name, x.CustomerType, x.Age));

        config.NewConfig<Models.Customer, CustomerDto>()
            .ConstructUsing(x => new CustomerDto(x.Id.Value, x.Name!.Value, x.CustomerType, x.Age!.Value));
    }
}
