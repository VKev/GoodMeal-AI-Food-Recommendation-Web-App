using Application.Prompt.Commands;
using FluentValidation;

namespace Application.Prompt.Validator;

public class CreatePromptSessionValidator : AbstractValidator<CreatePromptSessionCommand>
{
    public CreatePromptSessionValidator()
    {
    }
}