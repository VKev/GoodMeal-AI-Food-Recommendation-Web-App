using MassTransit;
using Microsoft.Extensions.Logging;
using SharedLibrary.Contracts.SubscriptionPayment;
using Application.Services;

namespace Application.Consumers;

public class SubscriptionPaymentStatusConsumer : 
    IConsumer<SubscriptionPaymentSagaStart>,
    IConsumer<SubscriptionPaymentUrlCreatedEvent>,
    IConsumer<SubscriptionPaymentUrlCreationFailedEvent>,
    IConsumer<SubscriptionPaymentCompletedEvent>,
    IConsumer<SubscriptionPaymentFailedEvent>,
    IConsumer<UserSubscriptionActivatedEvent>,
    IConsumer<UserSubscriptionActivationFailedEvent>
{
    private readonly ILogger<SubscriptionPaymentStatusConsumer> _logger;
    private readonly ISubscriptionPaymentStatusService _statusService;

    public SubscriptionPaymentStatusConsumer(
        ILogger<SubscriptionPaymentStatusConsumer> logger,
        ISubscriptionPaymentStatusService statusService)
    {
        _logger = logger;
        _statusService = statusService;
    }

    public async Task Consume(ConsumeContext<SubscriptionPaymentSagaStart> context)
    {
        _logger.LogInformation("Creating initial payment status for CorrelationId {CorrelationId}", context.Message.CorrelationId);
        
        var orderId = $"SUB_{context.Message.CorrelationId:N}";
        await _statusService.CreateInitialStatusAsync(
            context.Message.CorrelationId,
            context.Message.UserId,
            context.Message.SubscriptionId,
            context.Message.Amount,
            context.Message.Currency,
            orderId);
    }

    public async Task Consume(ConsumeContext<SubscriptionPaymentUrlCreatedEvent> context)
    {
        _logger.LogInformation("Updating payment URL created for CorrelationId {CorrelationId}", context.Message.CorrelationId);
        
        await _statusService.UpdatePaymentUrlCreatedAsync(
            context.Message.CorrelationId,
            context.Message.PaymentUrl);
    }

    public async Task Consume(ConsumeContext<SubscriptionPaymentUrlCreationFailedEvent> context)
    {
        _logger.LogInformation("Updating payment URL creation failed for CorrelationId {CorrelationId}", context.Message.CorrelationId);
        
        await _statusService.UpdatePaymentUrlFailedAsync(
            context.Message.CorrelationId,
            context.Message.Reason);
    }

    public async Task Consume(ConsumeContext<SubscriptionPaymentCompletedEvent> context)
    {
        _logger.LogInformation("Updating payment completed for CorrelationId {CorrelationId}", context.Message.CorrelationId);
        
        await _statusService.UpdatePaymentCompletedAsync(
            context.Message.CorrelationId,
            context.Message.TransactionId,
            context.Message.CompletedAt);
    }

    public async Task Consume(ConsumeContext<SubscriptionPaymentFailedEvent> context)
    {
        _logger.LogInformation("Updating payment failed for CorrelationId {CorrelationId}", context.Message.CorrelationId);
        
        await _statusService.UpdatePaymentFailedAsync(
            context.Message.CorrelationId,
            context.Message.Reason,
            context.Message.FailedAt);
    }

    public async Task Consume(ConsumeContext<UserSubscriptionActivatedEvent> context)
    {
        _logger.LogInformation("Updating subscription activated for CorrelationId {CorrelationId}", context.Message.CorrelationId);
        
        await _statusService.UpdateSubscriptionActivatedAsync(context.Message.CorrelationId);
    }

    public async Task Consume(ConsumeContext<UserSubscriptionActivationFailedEvent> context)
    {
        _logger.LogInformation("Updating subscription activation failed for CorrelationId {CorrelationId}", context.Message.CorrelationId);
        
        await _statusService.UpdateSubscriptionActivationFailedAsync(
            context.Message.CorrelationId,
            context.Message.Reason);
    }
} 