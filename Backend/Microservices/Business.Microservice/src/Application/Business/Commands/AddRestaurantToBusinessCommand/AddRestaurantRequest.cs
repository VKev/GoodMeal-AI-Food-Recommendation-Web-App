namespace Application.Business.Commands.AddRestaurantToBusinessCommand;

public sealed record AddRestaurantRequest(
    string Name,
    string? Address,
    string? Phone
); 