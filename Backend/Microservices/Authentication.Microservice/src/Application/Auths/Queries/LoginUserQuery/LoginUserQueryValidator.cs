using FluentValidation;

namespace Application.Auths.Commands;

public class LoginUserQueryValidator : AbstractValidator<LoginUserQuery>
{
    public LoginUserQueryValidator()
    {
        RuleFor(x => x.Email).NotEmpty().MaximumLength(70).EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MaximumLength(70);
    }
}