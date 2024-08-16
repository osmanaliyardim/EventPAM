using EventPAM.BuildingBlocks.Core.Model;

namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;

public record OperationClaim : Entity<Guid>, IVersion
{
    public string Name { get; set; } = default!;

    public OperationClaim() { }

    public OperationClaim(string name) : this()
    {
        Name = name;
    }
}
