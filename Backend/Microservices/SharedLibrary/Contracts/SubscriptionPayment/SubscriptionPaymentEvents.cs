namespace SharedLibrary.Contracts.SubscriptionPayment;

// Saga start event - triggered when user registers for subscription
public class SubscriptionPaymentSagaStart
{
    public Guid CorrelationId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid SubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "VND";
    public string OrderDescription { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
}

// Event to create payment URL in Payment microservice
public class CreateSubscriptionPaymentUrlEvent
{
    public Guid CorrelationId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid SubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "VND";
    public string OrderDescription { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
}

// Success event from Payment microservice - payment URL created
public class SubscriptionPaymentUrlCreatedEvent
{
    public Guid CorrelationId { get; set; }
    public string PaymentUrl { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
}

// Failure event from Payment microservice - payment URL creation failed
public class SubscriptionPaymentUrlCreationFailedEvent
{
    public Guid CorrelationId { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}

// Event triggered when payment is completed successfully (from VnPay callback)
public class SubscriptionPaymentCompletedEvent
{
    public Guid CorrelationId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid SubscriptionId { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public DateTime CompletedAt { get; set; }
}

// Event triggered when payment fails
public class SubscriptionPaymentFailedEvent
{
    public Guid CorrelationId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid SubscriptionId { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime FailedAt { get; set; }
}

// Event to activate user subscription after successful payment
public class ActivateUserSubscriptionEvent
{
    public Guid CorrelationId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid SubscriptionId { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public DateTime ActivatedAt { get; set; }
}

// Success event when user subscription is activated
public class UserSubscriptionActivatedEvent
{
    public Guid CorrelationId { get; set; }
    public Guid UserSubscriptionId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid SubscriptionId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

// Failure event when user subscription activation fails
public class UserSubscriptionActivationFailedEvent
{
    public Guid CorrelationId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid SubscriptionId { get; set; }
    public string Reason { get; set; } = string.Empty;
} 