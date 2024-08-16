namespace EventPAM.Customer.Customers;

public record CustomerReadModel
{
    public required Guid Id { get; init; }

    public required Guid CustomerId { get; init; }

    public required string Name { get; init; }

    public required string Email { get; init; }

    public required Enums.CustomerType CustomerType { get; init; }

    public int Age { get; init; }

    public required bool IsDeleted { get; init; }
}
