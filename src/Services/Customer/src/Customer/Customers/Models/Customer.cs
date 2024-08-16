using EventPAM.BuildingBlocks.Core.Model;
using EventPAM.Customer.Customers.Enums;
using EventPAM.Customer.Customers.Features.CompletingRegisterCustomer.V1;
using EventPAM.Customer.Customers.ValueObjects;
using EventPAM.Customer.Identity.Consumers.Registering.V1;
using EventPAM.Customer.ValueObjects;

namespace EventPAM.Customer.Customers.Models;

public record Customer : Aggregate<CustomerId>
{
    public Name Name { get; private set; } = default!;

    public CustomerType CustomerType { get; private set; }

    public Age? Age { get; private set; }

    public void CompleteRegistrationCustomer(CustomerId id, Name name, CustomerType customerType, 
        Age age, bool isDeleted = false)
    {
        Id = id;
        Name = name;
        CustomerType = customerType;
        Age = age;
        IsDeleted = isDeleted;

        var @event = new CustomerRegistrationCompletedDomainEvent(Id, Name, CustomerType, Age, IsDeleted);

        AddDomainEvent(@event);
    }

    public static Customer Create(CustomerId id, string name, bool isDeleted = false)
    {
        var customer = new Customer { Id = id, Name = Name.Of(name), IsDeleted = isDeleted };

        var @event = new CustomerCreatedDomainEvent(customer.Id, customer.Name.Value, customer.IsDeleted);

        customer.AddDomainEvent(@event);

        return customer;
    }
}
