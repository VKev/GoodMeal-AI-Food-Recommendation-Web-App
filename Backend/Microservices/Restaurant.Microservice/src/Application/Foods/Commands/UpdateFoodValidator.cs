using FluentValidation;

namespace Application.Foods.Commands;

public sealed class UpdateFoodValidator : AbstractValidator<UpdateFoodCommand>
{
    public UpdateFoodValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id không được để trống.");

        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Tên món ăn không được vượt quá 100 ký tự.")
            .When(x => !string.IsNullOrWhiteSpace(x.Name));

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Giá phải lớn hơn 0.")
            .When(x => x.Price.HasValue);

        RuleFor(x => x.ImageUrl)
            .MaximumLength(300).WithMessage("Đường dẫn ảnh không được vượt quá 300 ký tự.")
            .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl));

        // Nếu muốn thêm check cho DisableAt (ví dụ không cho chọn thời điểm trong quá khứ)
        RuleFor(x => x.DisableAt)
            .GreaterThan(DateTime.UtcNow).WithMessage("Thời gian DisableAt phải lớn hơn thời gian hiện tại.")
            .When(x => x.DisableAt.HasValue);
    }
}