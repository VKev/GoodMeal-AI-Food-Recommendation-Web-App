using FluentValidation;

namespace Application.Auths.Commands;

public class LoginWithGitHubQueryValidator : AbstractValidator<LoginWithGitHubQuery>
{
    public LoginWithGitHubQueryValidator()
    {
        RuleFor(x => x.AccessToken).NotEmpty();
    }
} 