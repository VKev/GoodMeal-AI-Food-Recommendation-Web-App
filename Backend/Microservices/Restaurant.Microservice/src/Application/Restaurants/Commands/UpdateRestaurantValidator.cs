using FluentValidation;

namespace Application.Restaurants.Commands;

public class UpdateRestaurantValidator : AbstractValidator<UpdateRestaurantCommand>
{
    public UpdateRestaurantValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Name)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.Name));

        RuleFor(x => x.Address)
            .MaximumLength(255)
            .When(x => !string.IsNullOrWhiteSpace(x.Address));

        RuleFor(x => x.Phone)
            .Matches(@"^\+?\d{7,15}$") 
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x.Website)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrWhiteSpace(x.Website));

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .When(x => x.Latitude.HasValue);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .When(x => x.Longitude.HasValue);
    }
}