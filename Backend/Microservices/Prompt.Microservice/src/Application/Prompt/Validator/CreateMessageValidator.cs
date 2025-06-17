using Application.Prompt.Commands;
using FluentValidation;

namespace Application.Prompt.Validator;

public class CreateMessageValidator : AbstractValidator<CreateMessageCommand>
{
    public CreateMessageValidator()
    {
        RuleFor(x => x.Sender).NotEmpty();
        RuleFor(x => x.PromptMessage).NotEmpty();
    }
}