using FluentValidation;

namespace Application.RestaurantRatings.Commands;

public sealed class CreateRatingValidator : AbstractValidator<CreateRatingCommand>
{
    public CreateRatingValidator()
    {
        RuleFor(x => x.RestaurantId)
            .NotEmpty().WithMessage("RestaurantId không được để trống.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId không được để trống.");

        RuleFor(x => x.Comment)
            .MaximumLength(500).WithMessage("Comment không được vượt quá 500 ký tự.")
            .When(x => !string.IsNullOrWhiteSpace(x.Comment));

        RuleFor(x => x.ImageUrl)
            .MaximumLength(300).WithMessage("Đường dẫn ảnh không được vượt quá 300 ký tự.")
            .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl));
    }
}