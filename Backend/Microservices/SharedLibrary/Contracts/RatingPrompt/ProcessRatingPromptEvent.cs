namespace SharedLibrary.Contracts.RatingPrompt;

public class ProcessRatingPromptEvent
{
    public Guid CorrelationId { get; set; }
    public string Comment { get; set; }
    public Guid? UserId { get; set; }
    public Guid? RestaurantId { get; set; }
    public string? ImageUrl  { get; set; }

}