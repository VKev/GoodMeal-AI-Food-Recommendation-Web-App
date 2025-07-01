namespace Domain.Entities;

public class SubscriptionPaymentStatus
{
    public Guid Id { get; set; }
    public Guid CorrelationId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid SubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "VND";
    public string OrderId { get; set; } = string.Empty;
    public string CurrentState { get; set; } = string.Empty;
    public string? PaymentUrl { get; set; }
    public bool PaymentUrlCreated { get; set; }
    public bool PaymentCompleted { get; set; }
    public bool SubscriptionActivated { get; set; }
    public string? TransactionId { get; set; }
    public string? FailureReason { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
} 