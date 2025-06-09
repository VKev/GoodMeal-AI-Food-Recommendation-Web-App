namespace Application.Restaurants.Queries;

public sealed record GetRestaurantResponse(
    Guid Id,
    string Name,
    string? Address,
    string Phone,
    string? CreatedBy,
    DateTime? CreatedAt,
    DateTime? UpdatedAt,
    bool IsDisable,
    DateTime? DisableAt,
    string? DisableBy
);