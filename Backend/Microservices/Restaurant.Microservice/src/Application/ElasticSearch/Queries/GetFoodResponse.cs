namespace Application.ElasticSearch.Queries;

public sealed record GetFoodResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public bool? IsAvailable { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? RestaurantId { get; set; }
    public string? ImageUrl { get; set; }
}