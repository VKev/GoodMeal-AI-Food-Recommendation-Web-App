namespace SharedLibrary.Contracts.Restaurant;

public class GetRestaurantByIdRequest
{
    public Guid Id { get; set; }
}

public class GetRestaurantByIdResponse
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public RestaurantDto Restaurant { get; set; } = new();
}

public class GetRestaurantsByIdsRequest
{
    public List<Guid> Ids { get; set; } = new();
}

public class GetRestaurantsByIdsResponse
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public List<RestaurantDto> Restaurants { get; set; } = new();
}

public class RestaurantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
} 