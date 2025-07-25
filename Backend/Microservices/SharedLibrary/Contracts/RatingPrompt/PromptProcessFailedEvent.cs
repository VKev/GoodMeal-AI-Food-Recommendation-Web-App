namespace SharedLibrary.Contracts.RatingPrompt;

public class PromptProcessFailedEvent
{
    public Guid CorrelationId { get; set; }
    public string Reason { get; set; }
    public DateTime FailedAt { get; set; }
}