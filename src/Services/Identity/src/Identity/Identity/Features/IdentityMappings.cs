using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;
using EventPAM.Identity.Dtos;
using EventPAM.Identity.Identity.Features.Registering.V1;

namespace EventPAM.Identity.Identity.Features;

public class IdentityMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisteredRequest, RegisterCommand>()
            .ConstructUsing(x => new RegisterCommand(
                x.FirstName, x.LastName, x.Email,
                x.Password, x.ConfirmPassword, x.IPAddress)
            );

        config.NewConfig<RegisteredResult, RegisteredResponse>()
            .ConstructUsing(x => new RegisteredResponse(
                x.FirstName, x.LastName, x.Email,
                x.AccessToken.Token, x.RefreshToken.Token)
            );

        config.NewConfig<User, UserDto>()
            .ConstructUsing(x => new UserDto(
                x.FirstName, x.LastName, x.Email, x.AuthenticatorType.ToString(), 
                x.UserOperationClaims.Select(uoc => uoc.OperationClaim.Name).ToList())
            );
    }
}
