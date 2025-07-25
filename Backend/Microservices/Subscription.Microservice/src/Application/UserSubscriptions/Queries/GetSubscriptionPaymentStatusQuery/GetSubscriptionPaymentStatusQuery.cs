using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using MassTransit;
using SharedLibrary.Contracts.SubscriptionPayment;

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
    DateTime? CreatedAt,
    DateTime? CompletedAt
);

internal sealed class
    GetSubscriptionPaymentStatusQueryHandler : IQueryHandler<GetSubscriptionPaymentStatusQuery,
    GetSubscriptionPaymentStatusResponse>
{
    private readonly ILogger<GetSubscriptionPaymentStatusQueryHandler> _logger;
    private readonly ISubscriptionPaymentStatusRepository _paymentStatusRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public GetSubscriptionPaymentStatusQueryHandler(
        ILogger<GetSubscriptionPaymentStatusQueryHandler> logger,
        ISubscriptionPaymentStatusRepository paymentStatusRepository,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _paymentStatusRepository = paymentStatusRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<GetSubscriptionPaymentStatusResponse>> Handle(GetSubscriptionPaymentStatusQuery request,
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
                return Result.Failure<GetSubscriptionPaymentStatusResponse>(new Error("SubscriptionPayment.NotFound",
                    "Subscription payment process not found"));
            }

            // Create and publish event to check payment status in Payment microservice

            if (!paymentStatus.PaymentCompleted && paymentStatus.PaymentUrlCreated)
            {
                await _publishEndpoint.Publish(new CheckSubscriptionPaymentStatusEvent
                {
                    CorrelationId = request.CorrelationId,
                    OrderId = paymentStatus.OrderId,
                    TransactionDate = paymentStatus.TransactionId ?? string.Empty,
                    UserId = paymentStatus.UserId,
                    SubscriptionId = paymentStatus.SubscriptionId,
                    Amount = paymentStatus.Amount
                }, cancellationToken);
            }

            _logger.LogInformation(
                "Published CheckSubscriptionPaymentStatusEvent for CorrelationId {CorrelationId}, OrderId {OrderId}",
                request.CorrelationId, paymentStatus.OrderId);

            var response = new GetSubscriptionPaymentStatusResponse(
                paymentStatus.CorrelationId,
                paymentStatus.CurrentState,
                paymentStatus.PaymentUrl,
                paymentStatus.PaymentUrlCreated,
                paymentStatus.PaymentCompleted,
                paymentStatus.SubscriptionActivated,
                paymentStatus.FailureReason,
                paymentStatus.CreatedAt,
                paymentStatus.CompletedAt
            );

            _logger.LogInformation(
                "Successfully retrieved subscription payment status for CorrelationId {CorrelationId}",
                request.CorrelationId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error retrieving subscription payment status for CorrelationId {CorrelationId}",
                request.CorrelationId);
            return Result.Failure<GetSubscriptionPaymentStatusResponse>(new Error("InternalError",
                "An unexpected error occurred"));
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