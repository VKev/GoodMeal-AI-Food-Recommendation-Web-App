namespace Application.RestaurantRatings.Queries;

public sealed record GetRatingResponse(
    Guid Id,
    Guid RestaurantId,
    Guid UserId,
    int? Rating,
    string? Comment,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool IsDisable,
    DateTime? DisableAt,
    string? DisableBy
);