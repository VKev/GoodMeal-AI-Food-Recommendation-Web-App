using MassTransit;
using Microsoft.Extensions.Logging;
using SharedLibrary.Contracts.SubscriptionPayment;
using Application.Payments.Commands;
using MediatR;

namespace Application.Consumers;

public class CreateSubscriptionPaymentUrlConsumer : IConsumer<CreateSubscriptionPaymentUrlEvent>
{
    private readonly ILogger<CreateSubscriptionPaymentUrlConsumer> _logger;
    private readonly IMediator _mediator;

    public CreateSubscriptionPaymentUrlConsumer(
        ILogger<CreateSubscriptionPaymentUrlConsumer> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<CreateSubscriptionPaymentUrlEvent> context)
    {
        _logger.LogInformation(
            "CreateSubscriptionPaymentUrlConsumer: Received CreateSubscriptionPaymentUrlEvent for user {UserId} and subscription {SubscriptionId} with CorrelationId {CorrelationId}",
            context.Message.UserId, context.Message.SubscriptionId, context.Message.CorrelationId);

        try
        {
            // Create payment URL using existing command
            var command = new CreatePaymentUrlCommand(
                context.Message.Amount,
                context.Message.OrderDescription,
                context.Message.OrderId
            );

            var result = await _mediator.Send(command, context.CancellationToken);

            if (result.IsSuccess)
            {
                // Publish success event
                await context.Publish(new SubscriptionPaymentUrlCreatedEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    PaymentUrl = result.Value.PaymentUrl,
                    OrderId = result.Value.OrderId
                });

                _logger.LogInformation(
                    "Successfully created payment URL for subscription {SubscriptionId} and user {UserId}",
                    context.Message.SubscriptionId, context.Message.UserId);
            }
            else
            {
                // Publish failure event
                await context.Publish(new SubscriptionPaymentUrlCreationFailedEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    OrderId = context.Message.OrderId,
                    Reason = result.Error.Description
                });

                _logger.LogWarning(
                    "Failed to create payment URL for subscription {SubscriptionId} and user {UserId}: {Reason}",
                    context.Message.SubscriptionId, context.Message.UserId, result.Error.Description);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error creating payment URL for subscription {SubscriptionId} and user {UserId} with CorrelationId {CorrelationId}",
                context.Message.SubscriptionId, context.Message.UserId, context.Message.CorrelationId);

            await context.Publish(new SubscriptionPaymentUrlCreationFailedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                OrderId = context.Message.OrderId,
                Reason = ex.Message
            });
        }
    }
} 