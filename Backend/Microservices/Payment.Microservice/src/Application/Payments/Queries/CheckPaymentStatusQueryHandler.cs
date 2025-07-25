using Domain.Repositories;
using SharedLibrary.Common.ResponseModel;
using SharedLibrary.Common.Messaging;
using Microsoft.Extensions.Logging;

namespace Application.Payments.Queries;

public sealed record CheckPaymentStatusQuery(
    string OrderId,
    string TransactionDate
) : IQuery<CheckPaymentStatusResponse>;

public sealed record CheckPaymentStatusResponse(
    string OrderId,
    string Message,
    bool IsCompleted
);

internal sealed class
    CheckPaymentStatusQueryHandler : IQueryHandler<CheckPaymentStatusQuery, CheckPaymentStatusResponse>
{
    private readonly ILogger<CheckPaymentStatusQueryHandler> _logger;
    private readonly IVnpayRepository _vnpayRepository;

    public CheckPaymentStatusQueryHandler(
        ILogger<CheckPaymentStatusQueryHandler> logger,
        IVnpayRepository vnpayRepository)
    {
        _logger = logger;
        _vnpayRepository = vnpayRepository;
    }

    public async Task<Result<CheckPaymentStatusResponse>> Handle(CheckPaymentStatusQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Checking payment status for OrderId: {OrderId}", request.OrderId);

            var result = await _vnpayRepository.GetPaymentResult(request.OrderId, request.TransactionDate);

            return Result.Success(new CheckPaymentStatusResponse(OrderId: request.OrderId, Message: result.Description,
                IsCompleted: result.IsSuccess));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking payment status for OrderId: {OrderId}", request.OrderId);
            return Result.Failure<CheckPaymentStatusResponse>(
                new Error("PaymentStatus.CheckFailed", "Failed to check payment status"));
        }
    }
}