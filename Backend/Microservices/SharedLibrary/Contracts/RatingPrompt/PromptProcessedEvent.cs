namespace SharedLibrary.Contracts.RatingPrompt

{
    public class PromptProcessedEvent
    {
        public Guid CorrelationId { get; set; }
        public Guid RatingId { get; set; }
        public int AIScore { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
} 