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
    IConsumer<UserSubscriptionActivationFailedEvent>,
    IConsumer<SubscriptionPaymentStatusCheckedEvent>
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
        
        // Check if a record already exists (could have been created by RegisterSubscriptionCommand or by URL event)
        var existingStatus = await _statusService.GetByCorrelationIdAsync(context.Message.CorrelationId);
        if (existingStatus != null)
        {
            _logger.LogInformation("Payment status record already exists for CorrelationId {CorrelationId}, skipping creation", 
                context.Message.CorrelationId);
            return;
        }
        
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
            context.Message.PaymentUrl, context.Message.UrlCreatedAt);
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
    
    public async Task Consume(ConsumeContext<SubscriptionPaymentStatusCheckedEvent> context)
    {
        _logger.LogInformation("Received payment status check result for CorrelationId {CorrelationId}, OrderId {OrderId}, IsCompleted: {IsCompleted}", 
            context.Message.CorrelationId, 
            context.Message.OrderId, 
            context.Message.IsCompleted);
        
        if (context.Message.IsCompleted)
        {
            // If payment is completed, update status
            await _statusService.UpdatePaymentCompletedAsync(
                context.Message.CorrelationId,
                $"Payment check confirmed: {context.Message.Message}",
                context.Message.CheckedAt);
            
            _logger.LogInformation("Updated payment status to completed for CorrelationId {CorrelationId}", context.Message.CorrelationId);
        }
        else
        {
            // Log the status check but don't update the status unless it's a definitive failure
            _logger.LogInformation("Payment not completed yet for CorrelationId {CorrelationId}: {Message}", 
                context.Message.CorrelationId, 
                context.Message.Message);
        }
    }
} 