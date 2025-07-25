namespace Application.Restaurants.Queries;

public sealed record GetRestaurantResponse(
    Guid Id,
    string Name,
    string? Address,
    string Phone,
    string? PlaceLink,
    string? Website,
    string? Types,
    float Latitude,
    float Longitude,
    string? TimeZone,
    string? Description,
    string? ImageUrl,
    string? CreatedBy,
    DateTime? CreatedAt,
    DateTime? UpdatedAt,
    bool IsDisable,
    DateTime? DisableAt,
    string? DisableBy
);