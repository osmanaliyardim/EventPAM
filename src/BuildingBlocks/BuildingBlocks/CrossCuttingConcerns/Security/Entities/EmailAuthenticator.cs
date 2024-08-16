using EventPAM.BuildingBlocks.Core.Model;

namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;

public record EmailAuthenticator : Entity<Guid>, IVersion
{
    public Guid UserId { get; set; }

    public string? ActivationKey { get; set; }

    public bool IsVerified { get; set; }

    public virtual User User { get; set; } = default!;

    public EmailAuthenticator() { }

    public EmailAuthenticator(Guid id, Guid userId, string? activationKey, bool isVerified)
        : this()
    {
        Id = id;
        UserId = userId;
        ActivationKey = activationKey;
        IsVerified = isVerified;
    }
}
