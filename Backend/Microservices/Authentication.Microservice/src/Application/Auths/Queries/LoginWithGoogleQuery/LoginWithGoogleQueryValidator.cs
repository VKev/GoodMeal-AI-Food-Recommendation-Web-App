using FluentValidation;

namespace Application.Auths.Commands;

public class LoginWithGoogleQueryValidator : AbstractValidator<LoginWithGoogleQuery>
{
    public LoginWithGoogleQueryValidator()
    {
        RuleFor(x => x.IdToken).NotEmpty();
    }
} 