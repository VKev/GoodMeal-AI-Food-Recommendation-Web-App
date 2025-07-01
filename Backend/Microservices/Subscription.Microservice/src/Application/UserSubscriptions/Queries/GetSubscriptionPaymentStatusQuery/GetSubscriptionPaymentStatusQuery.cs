using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;

namespace Application.UserSubscriptions.Queries.GetSubscriptionPaymentStatusQuery;

public sealed record GetSubscriptionPaymentStatusQuery(
    Guid CorrelationId
) : IQuery<GetSubscriptionPaymentStatusResponse>;

public sealed record GetSubscriptionPaymentStatusResponse(
    Guid CorrelationId,
    string CurrentState,
    string? PaymentUrl,
    bool PaymentUrlCreated,
    bool PaymentCompleted,
    bool SubscriptionActivated,
    string? FailureReason,
    DateTime? CompletedAt
);

internal sealed class GetSubscriptionPaymentStatusQueryHandler : IQueryHandler<GetSubscriptionPaymentStatusQuery, GetSubscriptionPaymentStatusResponse>
{
    private readonly ILogger<GetSubscriptionPaymentStatusQueryHandler> _logger;
    private readonly ISubscriptionPaymentStatusRepository _paymentStatusRepository;

    public GetSubscriptionPaymentStatusQueryHandler(
        ILogger<GetSubscriptionPaymentStatusQueryHandler> logger,
        ISubscriptionPaymentStatusRepository paymentStatusRepository)
    {
        _logger = logger;
        _paymentStatusRepository = paymentStatusRepository;
    }

    public async Task<Result<GetSubscriptionPaymentStatusResponse>> Handle(GetSubscriptionPaymentStatusQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var paymentStatus = await _paymentStatusRepository.GetByCorrelationIdAsync(request.CorrelationId, cancellationToken);
            
            if (paymentStatus == null)
            {
                _logger.LogWarning("Subscription payment status not found for CorrelationId {CorrelationId}", request.CorrelationId);
                return Result.Failure<GetSubscriptionPaymentStatusResponse>(new Error("SubscriptionPayment.NotFound", "Subscription payment process not found"));
            }

            var response = new GetSubscriptionPaymentStatusResponse(
                paymentStatus.CorrelationId,
                paymentStatus.CurrentState,
                paymentStatus.PaymentUrl,
                paymentStatus.PaymentUrlCreated,
                paymentStatus.PaymentCompleted,
                paymentStatus.SubscriptionActivated,
                paymentStatus.FailureReason,
                paymentStatus.CompletedAt
            );

            _logger.LogInformation("Successfully retrieved subscription payment status for CorrelationId {CorrelationId}", request.CorrelationId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving subscription payment status for CorrelationId {CorrelationId}", request.CorrelationId);
            return Result.Failure<GetSubscriptionPaymentStatusResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}

public class GetSubscriptionPaymentStatusQueryValidator : AbstractValidator<GetSubscriptionPaymentStatusQuery>
{
    public GetSubscriptionPaymentStatusQueryValidator()
    {
        RuleFor(x => x.CorrelationId)
            .NotEmpty()
            .WithMessage("Correlation ID is required");
    }
} 