using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;

namespace Application.UserSubscriptions.Queries.GetPaymentUrlQuery;

public sealed record GetPaymentUrlQuery(
    Guid CorrelationId
) : IQuery<GetPaymentUrlResponse>;

public sealed record GetPaymentUrlResponse(
    string? PaymentUrl,
    bool PaymentUrlCreated
);

internal sealed class GetPaymentUrlQueryHandler : IQueryHandler<GetPaymentUrlQuery, GetPaymentUrlResponse>
{
    private readonly ILogger<GetPaymentUrlQueryHandler> _logger;
    private readonly ISubscriptionPaymentStatusRepository _paymentStatusRepository;

    public GetPaymentUrlQueryHandler(
        ILogger<GetPaymentUrlQueryHandler> logger,
        ISubscriptionPaymentStatusRepository paymentStatusRepository)
    {
        _logger = logger;
        _paymentStatusRepository = paymentStatusRepository;
    }

    public async Task<Result<GetPaymentUrlResponse>> Handle(GetPaymentUrlQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var paymentStatus =
                await _paymentStatusRepository.GetByCorrelationIdAsync(request.CorrelationId, cancellationToken);

            if (paymentStatus == null)
            {
                _logger.LogWarning("Subscription payment status not found for CorrelationId {CorrelationId}",
                    request.CorrelationId);
                return Result.Failure<GetPaymentUrlResponse>(new Error("SubscriptionPayment.NotFound",
                    "Subscription payment process not found"));
            }

            var response = new GetPaymentUrlResponse(
                paymentStatus.PaymentUrl,
                paymentStatus.PaymentUrlCreated
            );

            _logger.LogInformation(
                "Successfully retrieved payment URL for CorrelationId {CorrelationId}",
                request.CorrelationId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error retrieving payment URL for CorrelationId {CorrelationId}",
                request.CorrelationId);
            return Result.Failure<GetPaymentUrlResponse>(new Error("InternalError",
                "An unexpected error occurred"));
        }
    }
}

public class GetPaymentUrlQueryValidator : AbstractValidator<GetPaymentUrlQuery>
{
    public GetPaymentUrlQueryValidator()
    {
        RuleFor(x => x.CorrelationId)
            .NotEmpty()
            .WithMessage("Correlation ID is required");
    }
} 