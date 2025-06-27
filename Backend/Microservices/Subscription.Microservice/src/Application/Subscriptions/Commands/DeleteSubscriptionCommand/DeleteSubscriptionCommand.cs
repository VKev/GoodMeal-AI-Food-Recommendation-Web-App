using System.Security.Claims;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Domain.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Common;

namespace Application.Subscriptions.Commands.DeleteSubscriptionCommand;

public sealed record DeleteSubscriptionCommand(Guid SubscriptionId) : ICommand<DeleteSubscriptionResponse>;

public sealed record DeleteSubscriptionResponse(
    Guid Id,
    string Name,
    bool IsDisable,
    DateTime? DisableAt,
    string? DisableBy
);

internal sealed class DeleteSubscriptionCommandHandler : ICommandHandler<DeleteSubscriptionCommand, DeleteSubscriptionResponse>
{
    private readonly ILogger<DeleteSubscriptionCommandHandler> _logger;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DeleteSubscriptionCommandHandler(
        ILogger<DeleteSubscriptionCommandHandler> logger,
        ISubscriptionRepository subscriptionRepository,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _subscriptionRepository = subscriptionRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<DeleteSubscriptionResponse>> Handle(DeleteSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
            {
                _logger.LogWarning("User ID not found in claims");
                return Result.Failure<DeleteSubscriptionResponse>(new Error("Auth.Unauthorized",
                    "User not authenticated"));
            }

            var userId = userIdClaim.Value;
            var subscription = await _subscriptionRepository.GetByIdAsync(request.SubscriptionId);
            
            if (subscription is null)
            {
                _logger.LogWarning("Subscription with ID {SubscriptionId} not found", request.SubscriptionId);
                return Result.Failure<DeleteSubscriptionResponse>(new Error("Subscription.NotFound",
                    "Subscription not found"));
            }

            if (subscription.IsDisable == true)
            {
                _logger.LogWarning("Subscription {SubscriptionId} is already disabled", request.SubscriptionId);
                return Result.Failure<DeleteSubscriptionResponse>(new Error("Subscription.AlreadyDisabled",
                    "Subscription is already disabled"));
            }

            subscription.IsDisable = true;
            subscription.DisableAt = DateTime.UtcNow;
            subscription.DisableBy = userId;

            _subscriptionRepository.Update(subscription);

            var response = _mapper.Map<DeleteSubscriptionResponse>(subscription);

            _logger.LogInformation("Successfully disabled subscription {SubscriptionId} by user {UserId}", 
                request.SubscriptionId, userId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error disabling subscription {SubscriptionId}", request.SubscriptionId);
            return Result.Failure<DeleteSubscriptionResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}

public class DeleteSubscriptionCommandValidator : AbstractValidator<DeleteSubscriptionCommand>
{
    public DeleteSubscriptionCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .NotEmpty()
            .WithMessage("Subscription ID is required");
    }
} 