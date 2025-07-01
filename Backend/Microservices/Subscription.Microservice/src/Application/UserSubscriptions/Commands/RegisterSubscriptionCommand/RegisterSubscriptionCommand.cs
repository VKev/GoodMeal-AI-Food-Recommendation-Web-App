using System.Security.Claims;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Common;
using SharedLibrary.Common.Event;
using SharedLibrary.Contracts.SubscriptionPayment;

namespace Application.UserSubscriptions.Commands.RegisterSubscriptionCommand;

public sealed record RegisterSubscriptionCommand(
    Guid SubscriptionId
) : ICommand<RegisterSubscriptionResponse>;

public sealed record RegisterSubscriptionResponse(
    Guid CorrelationId,
    string UserId,
    Guid SubscriptionId,
    decimal Amount,
    string Currency,
    string OrderId,
    string Message
);

internal sealed class RegisterSubscriptionCommandHandler : ICommandHandler<RegisterSubscriptionCommand, RegisterSubscriptionResponse>
{
    private readonly ILogger<RegisterSubscriptionCommandHandler> _logger;
    private readonly IUserSubscriptionRepository _userSubscriptionRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEventUnitOfWork _events;

    public RegisterSubscriptionCommandHandler(
        ILogger<RegisterSubscriptionCommandHandler> logger,
        IUserSubscriptionRepository userSubscriptionRepository,
        ISubscriptionRepository subscriptionRepository,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IEventUnitOfWork events)
    {
        _logger = logger;
        _userSubscriptionRepository = userSubscriptionRepository;
        _subscriptionRepository = subscriptionRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _events = events;
    }

    public async Task<Result<RegisterSubscriptionResponse>> Handle(RegisterSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
            {
                _logger.LogWarning("User ID not found in claims");
                return Result.Failure<RegisterSubscriptionResponse>(new Error("Auth.Unauthorized", "User not authenticated"));
            }

            var userId = userIdClaim.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User context is missing for subscription registration");
                return Result.Failure<RegisterSubscriptionResponse>(new Error("Authorization.Failed",
                    "User context is required"));
            }

            var subscription = await _subscriptionRepository.GetByIdAsync(request.SubscriptionId);
            if (subscription is null)
            {
                _logger.LogWarning("Subscription with ID {SubscriptionId} not found", request.SubscriptionId);
                return Result.Failure<RegisterSubscriptionResponse>(new Error("Subscription.NotFound", "Subscription not found"));
            }

            if (!subscription.IsActive || subscription.IsDisable == true)
            {
                _logger.LogWarning("Subscription {SubscriptionId} is not available", request.SubscriptionId);
                return Result.Failure<RegisterSubscriptionResponse>(new Error("Subscription.NotAvailable", "Subscription is not available"));
            }

            var existingActiveSubscription = await _userSubscriptionRepository.GetActiveSubscriptionByUserIdAsync(userId);
            if (existingActiveSubscription != null)
            {
                _logger.LogWarning("User {UserId} already has an active subscription", userId);
                return Result.Failure<RegisterSubscriptionResponse>(new Error("UserSubscription.AlreadyActive",
                    "User already has an active subscription"));
            }

            var correlationId = Guid.NewGuid();
            var amount = subscription.Price;
            
            // Convert amount to VND if needed
            if (subscription.Currency.ToUpper() == "USD")
            {
                amount = amount * 24000; // Approximate USD to VND conversion rate
            }

            var orderDescription = $"Đăng ký gói {subscription.Name} - {subscription.DurationInMonths} tháng";

            // Start subscription payment saga
            _events.Add(new SubscriptionPaymentSagaStart
            {
                CorrelationId = correlationId,
                UserId = userId,
                SubscriptionId = request.SubscriptionId,
                Amount = amount,
                Currency = "VND",
                OrderDescription = orderDescription,
                RequestedAt = DateTime.UtcNow
            });

            var response = new RegisterSubscriptionResponse(
                correlationId,
                userId,
                request.SubscriptionId,
                amount,
                "VND",
                $"SUB_{correlationId:N}",
                "Subscription registration process started. Payment URL will be created."
            );

            _logger.LogInformation("Started subscription registration process for user {UserId} and subscription {SubscriptionId} with CorrelationId {CorrelationId}", 
                userId, request.SubscriptionId, correlationId);
            
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error registering subscription {SubscriptionId}", request.SubscriptionId);
            return Result.Failure<RegisterSubscriptionResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}

public class RegisterSubscriptionCommandValidator : AbstractValidator<RegisterSubscriptionCommand>
{
    public RegisterSubscriptionCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .NotEmpty()
            .WithMessage("Subscription ID is required");
    }
} 