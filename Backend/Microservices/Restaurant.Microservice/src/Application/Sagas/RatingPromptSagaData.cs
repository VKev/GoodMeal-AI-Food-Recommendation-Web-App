using MassTransit;

namespace Application.Sagas;

public class RatingPromptSagaData: SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = string.Empty;
    public Guid RatingId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? RestaurantId { get; set; }
    public string Comment { get; set; } = string.Empty;
    public int? AIScore { get; set; }
    public string? FailureReason { get; set; }
    public int Version { get; set; }
}