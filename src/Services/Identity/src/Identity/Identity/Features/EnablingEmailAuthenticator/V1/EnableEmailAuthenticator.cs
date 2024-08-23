using EventPAM.BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MimeKit;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Entities;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Enums;
using EventPAM.BuildingBlocks.Mailing;
using EventPAM.Identity.Identity.Services.AuthenticatorService;
using EventPAM.Identity.Identity.Services.UserService;
using System.Web;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Builder;
using EventPAM.Identity.Repositories;

namespace EventPAM.Identity.Identity.Features.EnablingEmailAuthenticator.V1;

public class EnableEmailAuthenticatorCommand : IRequest
{
    public Guid UserId { get; set; }

    public string VerifyEmailUrlPrefix { get; set; } = default!;
}

public record EnableEmailAuthenticatorRequest(Guid UserId, string VerifyEmailUrlPrefix);

public class EnableEmailAuthenticatorCommandValidator : AbstractValidator<EnableEmailAuthenticatorCommand>
{
    public EnableEmailAuthenticatorCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
                .NotNull()
                    .WithMessage("Please enter the UserId");

        RuleFor(x => x.VerifyEmailUrlPrefix)
            .NotEmpty()
                .WithMessage("Please enter the VerifyEmailUrlPrefix");
    }
}

public class EnableEmailAuthenticatorEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/identity/enable-email-authenticator",
            async ([AsParameters] EnableEmailAuthenticatorRequest request, IMediator mediator,
            IMapper mapper, CancellationToken cancellationToken) =>
            {
                var command = mapper.Map<EnableEmailAuthenticatorCommand>(request);

                await mediator.Send(command, cancellationToken);
            }
        )
        //.RequireAuthorization(nameof(ApiScope))
        .WithName("EnableEmailAuthenticator")
        .WithApiVersionSet(builder.NewApiVersionSet("Identity").Build())
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Enable Email Authenticator")
        .WithDescription("Enable Email Authenticator")
        .WithOpenApi()
        .HasApiVersion(1.0);

        return builder;
    }
}

internal class EnableEmailAuthenticatorCommandHandler : IRequestHandler<EnableEmailAuthenticatorCommand>
{
    private readonly IAuthenticatorService _authenticatorService;
    private readonly IEmailAuthenticatorRepository _emailAuthenticatorRepository;
    private readonly IMailService _mailService;
    private readonly IUserService _userService;

    public EnableEmailAuthenticatorCommandHandler(
        IUserService userService,
        IEmailAuthenticatorRepository emailAuthenticatorRepository,
        IMailService mailService,
        IAuthenticatorService authenticatorService
    )
    {
        _userService = userService;
        _emailAuthenticatorRepository = emailAuthenticatorRepository;
        _mailService = mailService;
        _authenticatorService = authenticatorService;
    }

    public async Task Handle(EnableEmailAuthenticatorCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var user = await _userService.GetById(request.UserId);

        user.AuthenticatorType = AuthenticatorType.Email;
        await _userService.Update(user);

        EmailAuthenticator emailAuthenticator = await _authenticatorService.CreateEmailAuthenticator(user);
        EmailAuthenticator addedEmailAuthenticator = await _emailAuthenticatorRepository.AddAsync(emailAuthenticator);

        var toEmailList = new List<MailboxAddress> { new(name: $"{user.FirstName} {user.LastName}", user.Email) };

        _mailService.SendMail(
            new Mail
            {
                ToList = toEmailList,
                Subject = "Verify Your Email - EventPAM",
                TextBody =
                    $"Click on the link to verify your email: {request.VerifyEmailUrlPrefix}" +
                    $"?ActivationKey={HttpUtility.UrlEncode(addedEmailAuthenticator.ActivationKey)}"
            }
        );
    }
}
