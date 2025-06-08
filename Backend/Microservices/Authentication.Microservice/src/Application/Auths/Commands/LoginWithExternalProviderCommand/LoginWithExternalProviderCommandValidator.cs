using FluentValidation;

namespace Application.Auths.Commands;

public class LoginWithExternalProviderCommandValidator : AbstractValidator<LoginWithExternalProviderCommand>
{
    public LoginWithExternalProviderCommandValidator()
    {
        RuleFor(x => x.IdentityToken).NotEmpty();
    }
} 