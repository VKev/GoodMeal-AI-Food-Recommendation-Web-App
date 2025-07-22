namespace Domain.Common.Dtos;

public class FoodElasticDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid RestaurantId { get; set; }
}