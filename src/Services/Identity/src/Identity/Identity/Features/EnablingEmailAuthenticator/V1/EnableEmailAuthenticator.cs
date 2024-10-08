﻿using EventPAM.BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MimeKit;
using EventPAM.BuildingBlocks.CrossCuttingConcerns.Security.Enums;
using EventPAM.BuildingBlocks.Mailing;
using EventPAM.Identity.Identity.Services.AuthenticatorService;
using EventPAM.Identity.Identity.Services.UserService;
using System.Web;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Builder;
using EventPAM.Identity.Repositories;
using Microsoft.Extensions.Configuration;
using EventPAM.BuildingBlocks;
using static EventPAM.Identity.Identity.Constants.Constants.Role;

namespace EventPAM.Identity.Identity.Features.EnablingEmailAuthenticator.V1;

public record EnableEmailAuthenticatorCommand(Guid UserId, string VerifyEmailUrlPrefix) : IRequest;

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

public class EnableEmailAuthenticatorEndpoint : BaseController, IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/identity/enable-email-authenticator",
            async (IMediator mediator, IMapper mapper, IHttpContextAccessor context,
            IConfiguration configuration, CancellationToken cancellationToken) =>
            {
                var apiDomain = configuration
                    .GetSection(EventPAMBase.Configs.API_CONFIG).GetValue<string>("APIDomain");

                var command = new EnableEmailAuthenticatorCommand
                    (GetUserIdFromRequest(context), $"{apiDomain}/identity/verify-email-authenticator");

                await mediator.Send(command, cancellationToken);

                return Ok();
            }
        )
        .RequireAuthorization(policy => policy.RequireRole([Admin, Customer, EventManager]))
        .WithName("EnableEmailAuthenticator")
        .WithApiVersionSet(builder.NewApiVersionSet("Identity").Build())
        .Produces(StatusCodes.Status200OK)
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

        var emailAuthenticator = await _authenticatorService.CreateEmailAuthenticator(user);
        var addedEmailAuthenticator = await _emailAuthenticatorRepository.AddAsync(emailAuthenticator);

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
