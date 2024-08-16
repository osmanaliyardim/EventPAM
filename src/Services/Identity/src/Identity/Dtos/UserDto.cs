namespace EventPAM.Identity.Dtos;

public record UserDto(string FirstName, string LastName, string Email, 
    string AuthenticatorType, List<string> Roles);
