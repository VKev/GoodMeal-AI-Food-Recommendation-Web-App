using Domain.Repositories;
using SharedLibrary.Common.ResponseModel;
using SharedLibrary.Common.Messaging;
using Microsoft.Extensions.Logging;
using MassTransit;
using SharedLibrary.Contracts.SubscriptionPayment;

namespace Application.Payments.Queries;

public sealed record CheckPaymentStatusQuery(
    string OrderId
) : IQuery<CheckPaymentStatusResponse>;

public sealed record CheckPaymentStatusResponse(
    string OrderId,
    string Status,
    string Message,
    bool IsCompleted,
    decimal? Amount,
    string? TransactionId,
    DateTime? CompletedAt
);

internal sealed class
    CheckPaymentStatusQueryHandler : IQueryHandler<CheckPaymentStatusQuery, CheckPaymentStatusResponse>
{
    private readonly ILogger<CheckPaymentStatusQueryHandler> _logger;
    private readonly IVnpayRepository _vnpayRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public CheckPaymentStatusQueryHandler(
        ILogger<CheckPaymentStatusQueryHandler> logger,
        IVnpayRepository vnpayRepository,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _vnpayRepository = vnpayRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<CheckPaymentStatusResponse>> Handle(CheckPaymentStatusQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Checking payment status for OrderId: {OrderId}", request.OrderId);

            bool isSubscriptionPayment = request.OrderId.StartsWith("SUB_");

            if (isSubscriptionPayment)
            {
                return await HandleSubscriptionPaymentStatus(request.OrderId, cancellationToken);
            }
            else
            {
                return await HandleRegularPaymentStatus(request.OrderId, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking payment status for OrderId: {OrderId}", request.OrderId);
            return Result.Failure<CheckPaymentStatusResponse>(
                new Error("PaymentStatus.CheckFailed", "Failed to check payment status"));
        }
    }

    private async Task<Result<CheckPaymentStatusResponse>> HandleSubscriptionPaymentStatus(string orderId,
        CancellationToken cancellationToken)
    {
        try
        {
            var correlationIdString = orderId.Substring(4);
            if (!Guid.TryParse(correlationIdString, out var correlationId))
            {
                return Result.Failure<CheckPaymentStatusResponse>(
                    new Error("PaymentStatus.InvalidOrderId", "Invalid subscription order ID format"));
            }

            var response = new CheckPaymentStatusResponse(
                OrderId: orderId,
                Status: "CHECKING",
                Message:
                "Subscription payment status is managed by saga. Check callback endpoints or wait for payment completion notifications.",
                IsCompleted: false,
                Amount: null,
                TransactionId: null,
                CompletedAt: null
            );

            _logger.LogInformation("Subscription payment status check completed for OrderId: {OrderId}", orderId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling subscription payment status for OrderId: {OrderId}", orderId);
            throw;
        }
    }

    private async Task<Result<CheckPaymentStatusResponse>> HandleRegularPaymentStatus(string orderId,
        CancellationToken cancellationToken)
    {
        try
        {
            var transactionDate = DateTime.UtcNow.ToString("yyyyMMdd");
            var vnpayResult = await _vnpayRepository.QueryPaymentStatusAsync(orderId, transactionDate);

            var response = new CheckPaymentStatusResponse(
                OrderId: orderId,
                Status: vnpayResult.Status,
                Message: vnpayResult.Message,
                IsCompleted: vnpayResult.IsSuccess,
                Amount: vnpayResult.Amount,
                TransactionId: vnpayResult.TransactionId,
                CompletedAt: vnpayResult.TransactionDate
            );

            _logger.LogInformation("Regular payment status check completed for OrderId: {OrderId}, Status: {Status}",
                orderId, vnpayResult.Status);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling regular payment status for OrderId: {OrderId}", orderId);
            throw;
        }
    }
}