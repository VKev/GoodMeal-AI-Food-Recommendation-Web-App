using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using Domain.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Application.UserSubscriptions.Queries.GetAllSubscriptionPaymentStatusQuery;

public sealed record GetAllSubscriptionPaymentStatusQuery() : IQuery<GetAllSubscriptionPaymentStatusResponse>;

public sealed record SubscriptionPaymentStatusInfo(
    Guid CorrelationId,
    Guid SubscriptionId,
    decimal Amount,
    string Currency,
    string OrderId,
    string CurrentState,
    string? PaymentUrl,
    bool PaymentUrlCreated,
    bool PaymentCompleted,
    bool SubscriptionActivated,
    string? TransactionId,
    string? FailureReason,
    DateTime? CompletedAt,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public sealed record GetAllSubscriptionPaymentStatusResponse(
    IEnumerable<SubscriptionPaymentStatusInfo> PaymentStatuses
);

internal sealed class GetAllSubscriptionPaymentStatusQueryHandler : IQueryHandler<GetAllSubscriptionPaymentStatusQuery, GetAllSubscriptionPaymentStatusResponse>
{
    private readonly ILogger<GetAllSubscriptionPaymentStatusQueryHandler> _logger;
    private readonly ISubscriptionPaymentStatusRepository _paymentStatusRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetAllSubscriptionPaymentStatusQueryHandler(
        ILogger<GetAllSubscriptionPaymentStatusQueryHandler> logger,
        ISubscriptionPaymentStatusRepository paymentStatusRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _paymentStatusRepository = paymentStatusRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<GetAllSubscriptionPaymentStatusResponse>> Handle(GetAllSubscriptionPaymentStatusQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
            {
                _logger.LogWarning("User ID not found in claims");
                return Result.Failure<GetAllSubscriptionPaymentStatusResponse>(new Error("Auth.Unauthorized", "User not authenticated"));
            }

            var userId = userIdClaim.Value;
            var paymentStatuses = await _paymentStatusRepository.GetByUserIdAsync(userId, cancellationToken);

            var result = paymentStatuses.Select(p => new SubscriptionPaymentStatusInfo(
                p.CorrelationId,
                p.SubscriptionId,
                p.Amount,
                p.Currency,
                p.OrderId,
                p.CurrentState,
                p.PaymentUrl,
                p.PaymentUrlCreated,
                p.PaymentCompleted,
                p.SubscriptionActivated,
                p.TransactionId,
                p.FailureReason,
                p.CompletedAt,
                p.CreatedAt,
                p.UpdatedAt
            ));

            return Result.Success(new GetAllSubscriptionPaymentStatusResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving all subscription payment statuses for user");
            return Result.Failure<GetAllSubscriptionPaymentStatusResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}

public class GetAllSubscriptionPaymentStatusQueryValidator : AbstractValidator<GetAllSubscriptionPaymentStatusQuery>
{
    public GetAllSubscriptionPaymentStatusQueryValidator()
    {
        // No parameters to validate
    }
} 