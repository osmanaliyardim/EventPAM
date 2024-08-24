using EventPAM.BuildingBlocks.Web;
using EventPAM.Identity.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using static EventPAM.Identity.Identity.Constants.Constants.Role;

namespace EventPAM.Identity.Identity.Features.VerifyingEmailAuthenticator.V1;

public record VerifyEmailAuthenticatorCommand(string ActivationKey) : IRequest;

public record VerifyEmailAuthenticatorRequest(string ActivationKey);

public class VerifyEmailAuthenticatorEndpoint : BaseController, IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/identity/verify-email-authenticator",
            async ([AsParameters] VerifyEmailAuthenticatorRequest request, IMediator mediator, 
            IMapper mapper, CancellationToken cancellationToken) =>
            {
                var command = mapper.Map<VerifyEmailAuthenticatorCommand>(request);

                await mediator.Send(command, cancellationToken);

                return Ok();
            }
        )
        .RequireAuthorization(policy => policy.RequireRole([Admin, Customer, EventManager]))
        .WithName("VerifyEmailAuthenticator")
        .WithApiVersionSet(builder.NewApiVersionSet("Identity").Build())
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Verify Email Authenticator")
        .WithDescription("Verify Email Authenticator")
        .WithOpenApi()
        .HasApiVersion(1.0);

        return builder;
    }
}

internal class VerifyEmailAuthenticatorCommandHandler : IRequestHandler<VerifyEmailAuthenticatorCommand>
{
    private readonly IEmailAuthenticatorRepository _emailAuthenticatorRepository;

    public VerifyEmailAuthenticatorCommandHandler
        (IEmailAuthenticatorRepository emailAuthenticatorRepository)
    {
        _emailAuthenticatorRepository = emailAuthenticatorRepository;
    }

    public async Task Handle(VerifyEmailAuthenticatorCommand request, CancellationToken cancellationToken)
    {
        var emailAuthenticator = await _emailAuthenticatorRepository
            .GetAsync(e => e.ActivationKey == request.ActivationKey, cancellationToken: cancellationToken)
                ?? throw new EmailAuthNotFoundException();

        if (emailAuthenticator.ActivationKey is null)
            throw new EmailAuthKeyNotFoundException();

        emailAuthenticator.ActivationKey = null;
        emailAuthenticator.IsVerified = true;

        await _emailAuthenticatorRepository.UpdateAsync(emailAuthenticator);
    }
}
