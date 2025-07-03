using MassTransit;

namespace Application.Sagas;

public class SubscriptionPaymentSagaData : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = string.Empty;
    
    // User and Subscription info
    public string UserId { get; set; } = string.Empty;
    public Guid SubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "VND";
    public DateTime RequestedAt { get; set; }
    
    // Payment info
    public string OrderId { get; set; } = string.Empty;
    public string PaymentUrl { get; set; } = string.Empty;
    public bool PaymentUrlCreated { get; set; }
    public bool PaymentCompleted { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public DateTime? CompletedAt { get; set; }
    
    // Subscription activation info
    public bool SubscriptionActivated { get; set; }
    public Guid? UserSubscriptionId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    // Error handling
    public string? FailureReason { get; set; }
    public DateTime? FailedAt { get; set; }
    public int RetryCount { get; set; }
    
    public int Version { get; set; }
} 