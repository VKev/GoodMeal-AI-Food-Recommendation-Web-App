using FluentValidation;

namespace Application.Prompt.Commands;

public class CreatePromptSessionValidator : AbstractValidator<CreatePromptSessionCommand>
{
    public CreatePromptSessionValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}