using EventPAM.BuildingBlocks.Core.Model;

namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;

public record RefreshToken : Entity<Guid>, IVersion
{
    public Guid UserId { get; set; }

    public string Token { get; set; } = default!;

    public DateTime Expires { get; set; }

    public DateTime Created { get; set; }

    public string? CreatedByIp { get; set; }

    public DateTime? Revoked { get; set; }

    public string? RevokedByIp { get; set; }

    public string? ReplacedByToken { get; set; }

    public string? ReasonRevoked { get; set; }

    public virtual User User { get; set; } = default!;

    public RefreshToken() { }

    public RefreshToken(
        Guid id,
        string token,
        DateTime expires,
        DateTime created,
        string createdByIp,
        DateTime? revoked,
        string revokedByIp,
        string replacedByToken,
        string reasonRevoked
    ) 
    {
        Id = id;
        Token = token;
        Expires = expires;
        Created = created;
        CreatedByIp = createdByIp;
        Revoked = revoked;
        RevokedByIp = revokedByIp;
        ReplacedByToken = replacedByToken;
        ReasonRevoked = reasonRevoked;
    }
}
