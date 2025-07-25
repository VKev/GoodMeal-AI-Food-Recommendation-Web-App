using MassTransit;
using Microsoft.Extensions.Logging;
using SharedLibrary.Contracts.SubscriptionPayment;
using Application.Payments.Queries;
using MediatR;

namespace Application.Consumers;

public class CheckSubscriptionPaymentStatusConsumer : IConsumer<CheckSubscriptionPaymentStatusEvent>
{
    private readonly ILogger<CheckSubscriptionPaymentStatusConsumer> _logger;
    private readonly IMediator _mediator;

    public CheckSubscriptionPaymentStatusConsumer(
        ILogger<CheckSubscriptionPaymentStatusConsumer> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<CheckSubscriptionPaymentStatusEvent> context)
    {
        _logger.LogInformation(
            "CheckSubscriptionPaymentStatusConsumer: Received CheckSubscriptionPaymentStatusEvent for OrderId {OrderId} with CorrelationId {CorrelationId}",
            context.Message.OrderId, context.Message.CorrelationId);

        try
        {
            // Check payment status using existing query
            var query = new CheckPaymentStatusQuery(context.Message.OrderId, context.Message.TransactionDate);

            var result = await _mediator.Send(query, context.CancellationToken);

            if (result.IsSuccess)
            {
                _logger.LogInformation(
                    "Successfully checked payment status for OrderId {OrderId}. Is completed: {IsCompleted}",
                    context.Message.OrderId, result.Value.IsCompleted);

                if (result.Value.IsCompleted)
                {
                    await context.Publish(new SubscriptionPaymentCompletedEvent
                    {
                        CorrelationId = context.Message.CorrelationId,
                        UserId = context.Message.UserId,
                        SubscriptionId = context.Message.SubscriptionId,
                        OrderId = context.Message.OrderId,
                        Amount = context.Message.Amount,
                        TransactionId = result.Value.OrderId,
                        CompletedAt = DateTime.UtcNow
                    });

                    _logger.LogInformation(
                        "Published SubscriptionPaymentCompletedEvent for OrderId {OrderId} with CorrelationId {CorrelationId}",
                        context.Message.OrderId, context.Message.CorrelationId);
                }
                else
                {
                    // If payment is not completed but check was successful, still provide status checked event
                    await context.Publish(new SubscriptionPaymentStatusCheckedEvent
                    {
                        CorrelationId = context.Message.CorrelationId,
                        OrderId = context.Message.OrderId,
                        IsCompleted = false,
                        Message = result.Value.Message,
                        CheckedAt = DateTime.UtcNow
                    });
                }
            }
            else
            {
                _logger.LogWarning(
                    "Failed to check payment status for OrderId {OrderId}: {Reason}",
                    context.Message.OrderId, result.Error.Description);

                // If check payment status failed with a definitive failure
                await context.Publish(new SubscriptionPaymentFailedEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    UserId = context.Message.UserId,
                    SubscriptionId = context.Message.SubscriptionId,
                    OrderId = context.Message.OrderId,
                    Reason = result.Error.Description,
                    FailedAt = DateTime.UtcNow
                });
                
                _logger.LogInformation(
                    "Published SubscriptionPaymentFailedEvent for OrderId {OrderId} with CorrelationId {CorrelationId}",
                    context.Message.OrderId, context.Message.CorrelationId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error checking payment status for OrderId {OrderId} with CorrelationId {CorrelationId}",
                context.Message.OrderId, context.Message.CorrelationId);

            // Send event with error information
            await context.Publish(new SubscriptionPaymentStatusCheckedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                OrderId = context.Message.OrderId,
                IsCompleted = false,
                Message = $"Error checking payment status: {ex.Message}",
                CheckedAt = DateTime.UtcNow
            });
        }
    }
} 