namespace SharedLibrary.Contracts.RatingPrompt;

public class RatingScoreReadyEvent
{
    public Guid CorrelationId { get; set; }

    public Guid? UserId { get; set; }
    public Guid? RestaurantId { get; set; }
    public string Comment { get; set; }
    public float AIScore { get; set; }
    public string? ImageUrl  { get; set; }
    public DateTime ProcessedAt { get; set; }
}