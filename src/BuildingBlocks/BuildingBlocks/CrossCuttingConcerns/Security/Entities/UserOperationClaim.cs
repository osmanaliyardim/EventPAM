using EventPAM.BuildingBlocks.Core.Model;

namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;

public record UserOperationClaim : Entity<Guid>, IVersion
{
    public Guid UserId { get; set; }

    public Guid OperationClaimId { get; set; }

    public virtual User User { get; set; } = default!;

    public virtual OperationClaim OperationClaim { get; set; } = default!;

    public UserOperationClaim() { }

    public UserOperationClaim(Guid userId, Guid operationClaimId)
        : this()
    {
        UserId = userId;
        OperationClaimId = operationClaimId;
    }
}
