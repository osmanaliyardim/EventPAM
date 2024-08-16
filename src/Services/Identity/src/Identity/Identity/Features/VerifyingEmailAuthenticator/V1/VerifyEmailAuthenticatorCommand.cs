using EventPAM.Identity.Repositories;

namespace EventPAM.Identity.Identity.Features.VerifyingEmailAuthenticator.V1;

public record VerifyEmailAuthenticatorCommand(string ActivationKey) : IRequest;

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
