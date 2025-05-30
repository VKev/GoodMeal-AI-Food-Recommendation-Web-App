using FluentValidation;

namespace Application.Auths.Commands;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().MaximumLength(70).EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(70);
        RuleFor(x => x.Name).NotEmpty().MinimumLength(6).MaximumLength(70);
    }
} 