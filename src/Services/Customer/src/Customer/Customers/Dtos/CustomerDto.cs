namespace EventPAM.Customer.Customers.Dtos;

public record CustomerDto
    (Guid Id, string Name, Enums.CustomerType CustomerType, int Age);
