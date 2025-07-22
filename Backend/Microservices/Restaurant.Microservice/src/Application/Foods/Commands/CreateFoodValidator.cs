using FluentValidation;

namespace Application.Foods.Commands;

public sealed class CreateFoodValidator : AbstractValidator<CreateFoodCommand>
{
    public CreateFoodValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên món ăn không được để trống.")
            .MaximumLength(100).WithMessage("Tên món ăn không được vượt quá 100 ký tự.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự.")
            .When(x => x.Description != null);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Giá phải lớn hơn 0.")
            .When(x => x.Price.HasValue);

        RuleFor(x => x.RestaurantId)
            .NotEmpty().WithMessage("RestaurantId không được để trống.");
        
        RuleFor(x => x.ImageUrl)
            .MaximumLength(300).WithMessage("Đường dẫn ảnh không được vượt quá 300 ký tự.")
            .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl));

        RuleFor(x => x.IsAvailable)
            .NotNull().WithMessage("Trạng thái IsAvailable không được null.");
    }
}