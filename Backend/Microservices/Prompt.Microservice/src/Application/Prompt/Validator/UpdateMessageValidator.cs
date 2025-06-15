using Application.Prompt.Commands;
using FluentValidation;

namespace Application.Prompt.Validator;

public class UpdateMessageValidator : AbstractValidator<UpdateMessageCommand>
{
    public UpdateMessageValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.PromptMessage).NotEmpty();
        RuleFor(x => x.ResponseMessage).NotEmpty();
    }
}