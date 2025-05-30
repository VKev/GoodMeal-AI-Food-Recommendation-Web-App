using FluentValidation;

namespace Application.Auths.Commands;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().MaximumLength(70).EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MaximumLength(70);
    }
}