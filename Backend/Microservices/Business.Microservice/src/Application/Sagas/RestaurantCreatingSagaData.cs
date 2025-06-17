using MassTransit;

namespace Application.Sagas;

public class RestaurantCreatingSagaData : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = string.Empty;
    public Guid BusinessId { get; set; }
    public Guid RestaurantId { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public bool RestaurantCreated { get; set; }
    public bool BusinessRestaurantCreated { get; set; }
    public string? FailureReason { get; set; }
    public int RetryCount { get; set; }
    public int Version { get; set; }
} 