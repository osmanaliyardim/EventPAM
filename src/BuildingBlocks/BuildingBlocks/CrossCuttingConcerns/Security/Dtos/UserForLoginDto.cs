namespace EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Dtos;

public class UserForLoginDto
{
    public string Email { get; set; } = default!;

    public string Password { get; set; } = default!;

    public string? AuthenticatorCode { get; set; }
}
