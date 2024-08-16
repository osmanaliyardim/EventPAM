using EventPAM.BuildingBlocks.Core.Model;

namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;

public record OtpAuthenticator : Entity<Guid>, IVersion
{
    public Guid UserId { get; set; }

    public byte[] SecretKey { get; set; } = default!;

    public bool IsVerified { get; set; }

    public virtual User User { get; set; } = default!;

    public OtpAuthenticator() 
    { 
    
    }

    public OtpAuthenticator(Guid id, Guid userId, byte[] secretKey, bool isVerified)
        : this()
    {
        Id = id;
        UserId = userId;
        SecretKey = secretKey;
        IsVerified = isVerified;
    }
}
