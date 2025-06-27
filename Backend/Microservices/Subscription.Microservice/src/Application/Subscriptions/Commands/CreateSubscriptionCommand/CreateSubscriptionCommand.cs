using System.Security.Claims;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Common;

namespace Application.Subscriptions.Commands.CreateSubscriptionCommand;

public sealed record CreateSubscriptionCommand(
    string Name,
    string? Description,
    decimal Price,
    int DurationInMonths,
    string Currency = "USD"
) : ICommand<CreateSubscriptionResponse>;

public sealed record CreateSubscriptionResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    int DurationInMonths,
    string Currency,
    bool IsActive,
    DateTime? CreatedAt
);

internal sealed class CreateSubscriptionCommandHandler : ICommandHandler<CreateSubscriptionCommand, CreateSubscriptionResponse>
{
    private readonly ILogger<CreateSubscriptionCommandHandler> _logger;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateSubscriptionCommandHandler(
        ILogger<CreateSubscriptionCommandHandler> logger,
        ISubscriptionRepository subscriptionRepository,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _subscriptionRepository = subscriptionRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<CreateSubscriptionResponse>> Handle(CreateSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
            {
                _logger.LogWarning("User ID not found in claims");
                return Result.Failure<CreateSubscriptionResponse>(new Error("Auth.Unauthorized", "User not authenticated"));
            }

            var userId = userIdClaim.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User context is missing for subscription creation");
                return Result.Failure<CreateSubscriptionResponse>(new Error("Authorization.Failed",
                    "User context is required"));
            }

            var existsSubscription = await _subscriptionRepository.ExistsByNameAsync(request.Name);
            if (existsSubscription)
            {
                _logger.LogWarning("Subscription with name {Name} already exists", request.Name);
                return Result.Failure<CreateSubscriptionResponse>(new Error("Subscription.AlreadyExists",
                    "Subscription with this name already exists"));
            }

            var subscription = new Domain.Entities.Subscription
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                DurationInMonths = request.DurationInMonths,
                Currency = request.Currency,
                IsActive = true,
                IsDisable = false,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _subscriptionRepository.AddAsync(subscription, cancellationToken);

            var response = _mapper.Map<CreateSubscriptionResponse>(subscription);

            _logger.LogInformation("Successfully created subscription {SubscriptionId} by user {UserId}", subscription.Id, userId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating subscription");
            return Result.Failure<CreateSubscriptionResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}

public class CreateSubscriptionCommandValidator : AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
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