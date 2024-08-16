using EventPAM.BuildingBlocks.Core.Model;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Enums;

namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;

public record User : Entity<Guid>, IVersion
{
    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public string Email { get; set; } = default!;

    public byte[] PasswordSalt { get; set; } = default!;

    public byte[] PasswordHash { get; set; } = default!;

    public AuthenticatorType AuthenticatorType { get; set; }

    public virtual ICollection<UserOperationClaim> UserOperationClaims { get; set; } = default!;

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = default!;

    public User()
    {
        UserOperationClaims = new HashSet<UserOperationClaim>();
        RefreshTokens = new HashSet<RefreshToken>();
    }

    public User(
        Guid id,
        string firstName,
        string lastName,
        string email,
        byte[] passwordSalt,
        byte[] passwordHash,
        AuthenticatorType authenticatorType,
        bool isDeleted = false
    ) 
        : this()
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordSalt = passwordSalt;
        PasswordHash = passwordHash;
        AuthenticatorType = authenticatorType;
        IsDeleted = isDeleted;
    }
}
