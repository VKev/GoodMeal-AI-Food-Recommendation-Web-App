using System.Security.Claims;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Common;

namespace Application.UserSubscriptions.Queries.GetMySubscriptionQuery;

public sealed record GetMySubscriptionQuery() : IQuery<GetMySubscriptionResponse>;

public sealed record GetMySubscriptionResponse(
    Guid Id,
    string UserId,
    Guid SubscriptionId,
    string SubscriptionName,
    decimal SubscriptionPrice,
    int DurationInMonths,
    string Currency,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive,
    DateTime? CreatedAt
);

internal sealed class GetMySubscriptionQueryHandler : IQueryHandler<GetMySubscriptionQuery, GetMySubscriptionResponse>
{
    private readonly ILogger<GetMySubscriptionQueryHandler> _logger;
    private readonly IUserSubscriptionRepository _userSubscriptionRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetMySubscriptionQueryHandler(
        ILogger<GetMySubscriptionQueryHandler> logger,
        IUserSubscriptionRepository userSubscriptionRepository,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _userSubscriptionRepository = userSubscriptionRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<GetMySubscriptionResponse>> Handle(GetMySubscriptionQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
            {
                _logger.LogWarning("User ID not found in claims");
                return Result.Failure<GetMySubscriptionResponse>(new Error("Auth.Unauthorized", "User not authenticated"));
            }

            var userId = userIdClaim.Value;
            var userSubscription = await _userSubscriptionRepository.GetActiveSubscriptionByUserIdAsync(userId);
            
            if (userSubscription is null)
            {
                _logger.LogWarning("User {UserId} does not have an active subscription", userId);
                return Result.Failure<GetMySubscriptionResponse>(new Error("UserSubscription.NotFound", "No active subscription found"));
            }

            var response = new GetMySubscriptionResponse(
                userSubscription.Id,
                userSubscription.UserId,
                userSubscription.SubscriptionId,
                userSubscription.Subscription.Name,
                userSubscription.Subscription.Price,
                userSubscription.Subscription.DurationInMonths,
                userSubscription.Subscription.Currency,
                userSubscription.StartDate,
                userSubscription.EndDate,
                userSubscription.IsActive,
                userSubscription.CreatedAt
            );

            _logger.LogInformation("Successfully retrieved subscription for user {UserId}", userId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving user subscription");
            return Result.Failure<GetMySubscriptionResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
} 