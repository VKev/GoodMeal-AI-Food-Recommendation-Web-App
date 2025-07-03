using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Common;
using Application.Payments.Commands;
using Application.Payments.Queries;
using SharedLibrary.Contracts.SubscriptionPayment;
using MassTransit;
using VNPAY.NET.Utilities;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class PaymentController : ApiController
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IMediator mediator, IPublishEndpoint publishEndpoint, ILogger<PaymentController> logger) :
        base(mediator)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    [HttpPost("CreatePaymentUrl")]
    public async Task<IActionResult> CreatePaymentUrl(
        [FromBody] CreatePaymentUrlCommand request,
        CancellationToken cancellationToken)
    {
        var ipAddress = NetworkHelper.GetIpAddress(HttpContext);

        var command = new CreatePaymentUrlCommand(
            request.Amount,
            request.OrderDescription,
            request.OrderId,
            ipAddress
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
        _logger.LogInformation("Received IPN callback from VnPay");

        var command = new ProcessIpnCommand(Request.Query);

        var result = await _mediator.Send(command, cancellationToken);

        var aggregatedResult = ResultAggregator.Aggregate(result);
        if (aggregatedResult.IsFailure)
        {
            _logger.LogWarning("IPN processing failed: {Error}", aggregatedResult.Error);
            return HandleFailure(aggregatedResult);
        }

        _logger.LogInformation("IPN processed successfully. Payment success: {IsSuccess}", result.Value.IsSuccess);

        await HandleSubscriptionPaymentCompletion(Request.Query, result.Value.IsSuccess, cancellationToken);

        return Ok(aggregatedResult);
    }

    [HttpGet("Callback")]
    public async Task<IActionResult> Callback(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received user callback from VnPay");

        var query = new GetPaymentCallbackQuery(Request.Query);

        var result = await _mediator.Send(query, cancellationToken);

        var aggregatedResult = ResultAggregator.Aggregate(result);
        if (aggregatedResult.IsFailure)
        {
            _logger.LogWarning("Callback processing failed: {Error}", aggregatedResult.Error);
            return HandleFailure(aggregatedResult);
        }

        _logger.LogInformation("Callback processed successfully. Payment success: {IsSuccess}", result.Value.IsSuccess);

        await HandleSubscriptionPaymentCompletion(Request.Query, result.Value.IsSuccess, cancellationToken);

        return Ok(aggregatedResult);
    }

    [HttpGet("CheckStatus/{orderId}")]
    public async Task<IActionResult> CheckPaymentStatus(
        string orderId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received payment status check request for OrderId: {OrderId}", orderId);

        var query = new CheckPaymentStatusQuery(orderId);

        var result = await _mediator.Send(query, cancellationToken);

        var aggregatedResult = ResultAggregator.Aggregate(result);
        if (aggregatedResult.IsFailure)
        {
            _logger.LogWarning("Payment status check failed for OrderId: {OrderId}. Error: {Error}",
                orderId, aggregatedResult.Error);
            return HandleFailure(aggregatedResult);
        }

        _logger.LogInformation("Payment status check completed for OrderId: {OrderId}. Status: {Status}",
            orderId, result.Value.Status);

        return Ok(aggregatedResult);
    }

    [HttpGet("health")]
    public async Task<IActionResult> Health(CancellationToken cancellationToken)
    {
        return Ok();
    }

    private async Task HandleSubscriptionPaymentCompletion(IQueryCollection queryParameters, bool isSuccess,
        CancellationToken cancellationToken)
    {
        try
        {
            var orderId = queryParameters["vnp_TxnRef"].FirstOrDefault();
            var transactionId = queryParameters["vnp_TransactionNo"].FirstOrDefault();
            var amount = queryParameters["vnp_Amount"].FirstOrDefault();

            _logger.LogInformation(
                "Processing payment completion - OrderId: {OrderId}, TransactionId: {TransactionId}, Amount: {Amount}, Success: {Success}",
                orderId, transactionId, amount, isSuccess);

            // Check if this is a subscription payment (order ID starts with SUB_)
            if (string.IsNullOrEmpty(orderId) || !orderId.StartsWith("SUB_"))
            {
                _logger.LogDebug("Skipping non-subscription payment with OrderId: {OrderId}", orderId);
                return; // Not a subscription payment
            }

            // Extract correlation ID from order ID
            var correlationIdString = orderId.Substring(4); // Remove "SUB_" prefix
            if (!Guid.TryParse(correlationIdString, out var correlationId))
            {
                _logger.LogWarning("Invalid correlation ID in OrderId: {OrderId}", orderId);
                return;
            }

            _logger.LogInformation("Handling subscription payment completion for CorrelationId: {CorrelationId}",
                correlationId);

            if (isSuccess)
            {
                decimal.TryParse(amount, out var amountValue);

                var completedEvent = new SubscriptionPaymentCompletedEvent
                {
                    CorrelationId = correlationId,
                    UserId = string.Empty,
                    SubscriptionId = Guid.Empty,
                    OrderId = orderId,
                    Amount = amountValue,
                    TransactionId = transactionId ?? string.Empty,
                    CompletedAt = DateTime.UtcNow
                };

                await _publishEndpoint.Publish(completedEvent, cancellationToken);

                _logger.LogInformation(
                    "Published SubscriptionPaymentCompletedEvent for CorrelationId: {CorrelationId}, Amount: {Amount}",
                    correlationId, amountValue);
            }
            else
            {
                var failedEvent = new SubscriptionPaymentFailedEvent
                {
                    CorrelationId = correlationId,
                    UserId = string.Empty,
                    SubscriptionId = Guid.Empty,
                    OrderId = orderId,
                    Reason = "Payment processing failed",
                    FailedAt = DateTime.UtcNow
                };

                await _publishEndpoint.Publish(failedEvent, cancellationToken);

                _logger.LogWarning(
                    "Published SubscriptionPaymentFailedEvent for CorrelationId: {CorrelationId}, Reason: Payment processing failed",
                    correlationId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling subscription payment completion");
        }
    }
}