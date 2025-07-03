using System.Security.Claims;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Common;

namespace Application.Subscriptions.Commands.UpdateSubscriptionCommand;

public sealed record UpdateSubscriptionCommand(
    Guid SubscriptionId,
    string Name,
    string? Description,
    decimal Price,
    int DurationInMonths,
    string Currency
) : ICommand<UpdateSubscriptionResponse>;

public sealed record UpdateSubscriptionResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    int DurationInMonths,
    string Currency,
    bool IsActive,
    DateTime? UpdatedAt
);

internal sealed class UpdateSubscriptionCommandHandler : ICommandHandler<UpdateSubscriptionCommand, UpdateSubscriptionResponse>
{
    private readonly ILogger<UpdateSubscriptionCommandHandler> _logger;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateSubscriptionCommandHandler(
        ILogger<UpdateSubscriptionCommandHandler> logger,
        ISubscriptionRepository subscriptionRepository,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _subscriptionRepository = subscriptionRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<UpdateSubscriptionResponse>> Handle(UpdateSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
            {
                _logger.LogWarning("User ID not found in claims");
                return Result.Failure<UpdateSubscriptionResponse>(new Error("Auth.Unauthorized",
                    "User not authenticated"));
            }

            var userId = userIdClaim.Value;
            var subscription = await _subscriptionRepository.GetByIdAsync(request.SubscriptionId);
            
            if (subscription is null)
            {
                _logger.LogWarning("Subscription with ID {SubscriptionId} not found", request.SubscriptionId);
                return Result.Failure<UpdateSubscriptionResponse>(new Error("Subscription.NotFound",
                    "Subscription not found"));
            }

            if (subscription.IsDisable == true)
            {
                _logger.LogWarning("Cannot update disabled subscription {SubscriptionId}", request.SubscriptionId);
                return Result.Failure<UpdateSubscriptionResponse>(new Error("Subscription.Disabled",
                    "Cannot update a disabled subscription"));
            }

            subscription.Name = request.Name;
            subscription.Description = request.Description;
            subscription.Price = request.Price;
            subscription.DurationInMonths = request.DurationInMonths;
            subscription.Currency = request.Currency;
            subscription.UpdatedAt = DateTime.UtcNow;
            subscription.UpdatedBy = userId;

            _subscriptionRepository.Update(subscription);

            var response = _mapper.Map<UpdateSubscriptionResponse>(subscription);

            _logger.LogInformation("Successfully updated subscription {SubscriptionId} by user {UserId}", 
                request.SubscriptionId, userId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating subscription {SubscriptionId}", request.SubscriptionId);
            return Result.Failure<UpdateSubscriptionResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}

public class UpdateSubscriptionCommandValidator : AbstractValidator<UpdateSubscriptionCommand>
{
    public UpdateSubscriptionCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .NotEmpty()
            .WithMessage("Subscription ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Subscription name is required")
            .MaximumLength(200)
            .WithMessage("Subscription name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0");

        RuleFor(x => x.DurationInMonths)
            .GreaterThan(0)
            .WithMessage("Duration must be greater than 0 months");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required")
            .Length(3)
            .WithMessage("Currency must be 3 characters long");
    }
} 