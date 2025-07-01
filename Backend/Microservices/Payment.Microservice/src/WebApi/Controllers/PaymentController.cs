using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Common;
using Application.Payments.Commands;
using Application.Payments.Queries;
using SharedLibrary.Contracts.SubscriptionPayment;
using MassTransit;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class PaymentController : ApiController
{
    private readonly IPublishEndpoint _publishEndpoint;
    
    public PaymentController(IMediator mediator, IPublishEndpoint publishEndpoint) : base(mediator)
    {
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost("CreatePaymentUrl")]
    public async Task<IActionResult> CreatePaymentUrl(
        [FromBody] CreatePaymentUrlCommand request,
        CancellationToken cancellationToken)
    {
        var command = new CreatePaymentUrlCommand(
            request.Amount,
            request.OrderDescription,
            request.OrderId
        );

        var result = await _mediator.Send(command, cancellationToken);

        var aggregatedResult = ResultAggregator.Aggregate(result);
        if (aggregatedResult.IsFailure)
        {
            return HandleFailure(aggregatedResult);
        }

        return Ok(aggregatedResult);
    }

    [HttpGet("IpnAction")]
    public async Task<IActionResult> IpnAction(CancellationToken cancellationToken)
    {
        var command = new ProcessIpnCommand(Request.Query);

        var result = await _mediator.Send(command, cancellationToken);

        var aggregatedResult = ResultAggregator.Aggregate(result);
        if (aggregatedResult.IsFailure)
        {
            return HandleFailure(aggregatedResult);
        }

        // Handle subscription payment completion
        await HandleSubscriptionPaymentCompletion(Request.Query, result.Value.IsSuccess, cancellationToken);

        return Ok(aggregatedResult);
    }

    [HttpGet("Callback")]
    public async Task<IActionResult> Callback(CancellationToken cancellationToken)
    {
        var query = new GetPaymentCallbackQuery(Request.Query);

        var result = await _mediator.Send(query, cancellationToken);

        var aggregatedResult = ResultAggregator.Aggregate(result);
        if (aggregatedResult.IsFailure)
        {
            return HandleFailure(aggregatedResult);
        }

        return Ok(aggregatedResult);
    }

    [HttpGet("health")]
    public async Task<IActionResult> Health(CancellationToken cancellationToken)
    {
        return Ok();
    }

    private async Task HandleSubscriptionPaymentCompletion(IQueryCollection queryParameters, bool isSuccess, CancellationToken cancellationToken)
    {
        try
        {
            var orderId = queryParameters["vnp_TxnRef"].FirstOrDefault();
            var transactionId = queryParameters["vnp_TransactionNo"].FirstOrDefault();
            var amount = queryParameters["vnp_Amount"].FirstOrDefault();
            
            // Check if this is a subscription payment (order ID starts with SUB_)
            if (string.IsNullOrEmpty(orderId) || !orderId.StartsWith("SUB_"))
            {
                return; // Not a subscription payment
            }

            // Extract correlation ID from order ID
            var correlationIdString = orderId.Substring(4); // Remove "SUB_" prefix
            if (!Guid.TryParse(correlationIdString, out var correlationId))
            {
                return; // Invalid correlation ID
            }

            if (isSuccess)
            {
                // Parse amount (VnPay returns amount in smallest unit, so divide by 100)
                decimal.TryParse(amount, out var amountValue);
                amountValue = amountValue / 100;

                await _publishEndpoint.Publish(new SubscriptionPaymentCompletedEvent
                {
                    CorrelationId = correlationId,
                    UserId = string.Empty, // Will be populated by saga
                    SubscriptionId = Guid.Empty, // Will be populated by saga
                    OrderId = orderId,
                    Amount = amountValue,
                    TransactionId = transactionId ?? string.Empty,
                    CompletedAt = DateTime.UtcNow
                }, cancellationToken);
            }
            else
            {
                await _publishEndpoint.Publish(new SubscriptionPaymentFailedEvent
                {
                    CorrelationId = correlationId,
                    UserId = string.Empty, // Will be populated by saga
                    SubscriptionId = Guid.Empty, // Will be populated by saga
                    OrderId = orderId,
                    Reason = "Payment processing failed",
                    FailedAt = DateTime.UtcNow
                }, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            // Log error but don't throw to avoid affecting the main payment flow
            Console.WriteLine($"Error handling subscription payment completion: {ex.Message}");
        }
    }
}