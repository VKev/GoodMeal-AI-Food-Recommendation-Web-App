namespace Application.Foods.Queries;

public sealed record GetFoodResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    Guid RestaurantId,
    DateTime? CreatedAt,
    DateTime? UpdatedAt,
    bool IsDisable,
    DateTime? DisableAt
);