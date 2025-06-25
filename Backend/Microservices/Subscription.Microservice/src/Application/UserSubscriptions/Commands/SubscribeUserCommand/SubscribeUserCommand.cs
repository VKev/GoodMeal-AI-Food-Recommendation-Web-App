using System.Security.Claims;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Common;

namespace Application.UserSubscriptions.Commands.SubscribeUserCommand;

public sealed record SubscribeUserCommand(
    Guid SubscriptionId
) : ICommand<SubscribeUserResponse>;

public sealed record SubscribeUserResponse(
    Guid Id,
    string UserId,
    Guid SubscriptionId,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive,
    DateTime? CreatedAt
);

internal sealed class SubscribeUserCommandHandler : ICommandHandler<SubscribeUserCommand, SubscribeUserResponse>
{
    private readonly ILogger<SubscribeUserCommandHandler> _logger;
    private readonly IUserSubscriptionRepository _userSubscriptionRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SubscribeUserCommandHandler(
        ILogger<SubscribeUserCommandHandler> logger,
        IUserSubscriptionRepository userSubscriptionRepository,
        ISubscriptionRepository subscriptionRepository,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _userSubscriptionRepository = userSubscriptionRepository;
        _subscriptionRepository = subscriptionRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<SubscribeUserResponse>> Handle(SubscribeUserCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
            {
                _logger.LogWarning("User ID not found in claims");
                return Result.Failure<SubscribeUserResponse>(new Error("Auth.Unauthorized", "User not authenticated"));
            }

            var userId = userIdClaim.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User context is missing for subscription");
                return Result.Failure<SubscribeUserResponse>(new Error("Authorization.Failed",
                    "User context is required"));
            }

            var subscription = await _subscriptionRepository.GetByIdAsync(request.SubscriptionId);
            if (subscription is null)
            {
                _logger.LogWarning("Subscription with ID {SubscriptionId} not found", request.SubscriptionId);
                return Result.Failure<SubscribeUserResponse>(new Error("Subscription.NotFound", "Subscription not found"));
            }

            if (!subscription.IsActive || subscription.IsDisable == true)
            {
                _logger.LogWarning("Subscription {SubscriptionId} is not available", request.SubscriptionId);
                return Result.Failure<SubscribeUserResponse>(new Error("Subscription.NotAvailable", "Subscription is not available"));
            }

            var existingActiveSubscription = await _userSubscriptionRepository.GetActiveSubscriptionByUserIdAsync(userId);
            if (existingActiveSubscription != null)
            {
                _logger.LogWarning("User {UserId} already has an active subscription", userId);
                return Result.Failure<SubscribeUserResponse>(new Error("UserSubscription.AlreadyActive",
                    "User already has an active subscription"));
            }

            var startDate = DateTime.Now;
            var endDate = startDate.AddMonths(subscription.DurationInMonths);

            var userSubscription = new Domain.Entities.UserSubscription
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SubscriptionId = request.SubscriptionId,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = true,
                IsDisable = false,
                CreatedBy = userId,
                CreatedAt = startDate,
                UpdatedAt = startDate
            };

            await _userSubscriptionRepository.AddAsync(userSubscription, cancellationToken);

            var response = _mapper.Map<SubscribeUserResponse>(userSubscription);

            _logger.LogInformation("Successfully subscribed user {UserId} to subscription {SubscriptionId}", 
                userId, request.SubscriptionId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error subscribing user to subscription {SubscriptionId}", request.SubscriptionId);
            return Result.Failure<SubscribeUserResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}

public class SubscribeUserCommandValidator : AbstractValidator<SubscribeUserCommand>
{
    public SubscribeUserCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .NotEmpty()
            .WithMessage("Subscription ID is required");
    }
} 