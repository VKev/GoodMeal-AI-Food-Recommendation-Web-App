namespace SharedLibrary.Contracts.RatingPrompt;

public class RatingCreatedSagaStart
{
    public Guid CorrelationId { get; set; }
    public Guid RatingId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? RestaurantId { get; set; }
    public string Comment { get; set; }
    public DateTime? CreatedAt { get; set; }
}