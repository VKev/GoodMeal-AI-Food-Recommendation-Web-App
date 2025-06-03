using FluentValidation;

namespace Application.Users.Commands
{
    public class CreateUserValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Email).NotEmpty().MaximumLength(70).EmailAddress();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(70);
        }
    }
}