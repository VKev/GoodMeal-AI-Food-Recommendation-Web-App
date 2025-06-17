namespace Domain.Entities;

public sealed record RestaurantInfo(
    Guid Id,
    string Name,
    string? Description,
    string? Address,
    string? Phone,
    string? Email,
    bool IsActive,
    DateTime? CreatedAt
); 