using FluentValidation;

namespace Application.Auths.Commands;

public class LoginWithFacebookQueryValidator : AbstractValidator<LoginWithFacebookQuery>
{
    public LoginWithFacebookQueryValidator()
    {
        RuleFor(x => x.AccessToken).NotEmpty();
    }
} 