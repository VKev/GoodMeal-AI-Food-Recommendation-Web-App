using MassTransit;
using Microsoft.Extensions.Logging;
using SharedLibrary.Contracts.SubscriptionPayment;
using Domain.Repositories;
using Domain.Entities;
using SharedLibrary.Common;

namespace Application.Consumers;

public class ActivateUserSubscriptionConsumer : IConsumer<ActivateUserSubscriptionEvent>
{
    private readonly ILogger<ActivateUserSubscriptionConsumer> _logger;
    private readonly IUserSubscriptionRepository _userSubscriptionRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateUserSubscriptionConsumer(
        ILogger<ActivateUserSubscriptionConsumer> logger,
        IUserSubscriptionRepository userSubscriptionRepository,
        ISubscriptionRepository subscriptionRepository,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _userSubscriptionRepository = userSubscriptionRepository;
        _subscriptionRepository = subscriptionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<ActivateUserSubscriptionEvent> context)
    {
        _logger.LogInformation(
            "ActivateUserSubscriptionConsumer: Received ActivateUserSubscriptionEvent for user {UserId} and subscription {SubscriptionId} with CorrelationId {CorrelationId}",
            context.Message.UserId, context.Message.SubscriptionId, context.Message.CorrelationId);

        try
        {
            // Get subscription details
            var subscription = await _subscriptionRepository.GetByIdAsync(context.Message.SubscriptionId, context.CancellationToken);
            if (subscription == null)
            {
                await context.Publish(new UserSubscriptionActivationFailedEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    UserId = context.Message.UserId,
                    SubscriptionId = context.Message.SubscriptionId,
                    Reason = "Subscription not found"
                });
                return;
            }

            // Check if user already has an active subscription
            var existingSubscription = await _userSubscriptionRepository.GetActiveSubscriptionByUserIdAsync(context.Message.UserId);
            if (existingSubscription != null)
            {
                await context.Publish(new UserSubscriptionActivationFailedEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    UserId = context.Message.UserId,
                    SubscriptionId = context.Message.SubscriptionId,
                    Reason = "User already has an active subscription"
                });
                return;
            }

            // Create new user subscription
            var startDate = context.Message.ActivatedAt;
            var endDate = startDate.AddMonths(subscription.DurationInMonths);

            var userSubscription = new UserSubscription
            {
                Id = Guid.NewGuid(),
                UserId = context.Message.UserId,
                SubscriptionId = context.Message.SubscriptionId,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = true,
                IsDisable = false,
                CreatedBy = context.Message.UserId,
                CreatedAt = startDate,
                UpdatedAt = startDate
            };

            await _userSubscriptionRepository.AddAsync(userSubscription, context.CancellationToken);
            await _unitOfWork.SaveChangesAsync(context.CancellationToken);

            // Publish success event
            await context.Publish(new UserSubscriptionActivatedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                UserSubscriptionId = userSubscription.Id,
                UserId = context.Message.UserId,
                SubscriptionId = context.Message.SubscriptionId,
                StartDate = startDate,
                EndDate = endDate
            });

            _logger.LogInformation(
                "Successfully activated subscription {SubscriptionId} for user {UserId} with UserSubscriptionId {UserSubscriptionId}",
                context.Message.SubscriptionId, context.Message.UserId, userSubscription.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error activating subscription {SubscriptionId} for user {UserId} with CorrelationId {CorrelationId}",
                context.Message.SubscriptionId, context.Message.UserId, context.Message.CorrelationId);

            await context.Publish(new UserSubscriptionActivationFailedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                UserId = context.Message.UserId,
                SubscriptionId = context.Message.SubscriptionId,
                Reason = ex.Message
            });
        }
    }
} 