using MassTransit;
using Microsoft.Extensions.Logging;
using SharedLibrary.Contracts.RestaurantCreating;
using Domain.Repositories;
using Domain.Entities;
using SharedLibrary.Common;

namespace Application.Consumers;

public class CreateBusinessRestaurantConsumer : IConsumer<CreateBusinessRestaurantEvent>
{
    private readonly ILogger<CreateBusinessRestaurantConsumer> _logger;
    private readonly IBusinessRestaurantRepository _businessRestaurantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBusinessRestaurantConsumer(
        ILogger<CreateBusinessRestaurantConsumer> logger,
        IBusinessRestaurantRepository businessRestaurantRepository,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _businessRestaurantRepository = businessRestaurantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<CreateBusinessRestaurantEvent> context)
    {
        _logger.LogInformation(
            "CreateBusinessRestaurantConsumer: Received CreateBusinessRestaurantEvent for business {BusinessId} and restaurant {RestaurantId} with CorrelationId {CorrelationId}",
            context.Message.BusinessId, context.Message.RestaurantId, context.Message.CorrelationId);

        try
        {
            var existingRelationship = await _businessRestaurantRepository.GetByBusinessAndRestaurantIdAsync(
                context.Message.BusinessId, context.Message.RestaurantId, CancellationToken.None);

            if (existingRelationship != null)
            {
                await context.Publish(new BusinessRestaurantCreatedFailureEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    BusinessId = context.Message.BusinessId,
                    RestaurantId = context.Message.RestaurantId,
                    Reason = "Business-Restaurant relationship already exists"
                });
                return;
            }

            var businessRestaurant = new BusinessRestaurant
            {
                Id = Guid.NewGuid(),
                BusinessId = context.Message.BusinessId,
                RestaurantId = context.Message.RestaurantId,
                CreatedBy = context.Message.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDisable = false
            };

            await _businessRestaurantRepository.AddAsync(businessRestaurant, CancellationToken.None);
            ;
            await _unitOfWork.SaveChangesAsync();

            await context.Publish(new BusinessRestaurantCreatedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                BusinessId = context.Message.BusinessId,
                RestaurantId = context.Message.RestaurantId
            });

            _logger.LogInformation(
                "BusinessRestaurant relationship created successfully for business {BusinessId} and restaurant {RestaurantId} with CorrelationId {CorrelationId}",
                context.Message.BusinessId, context.Message.RestaurantId, context.Message.CorrelationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error creating BusinessRestaurant relationship for business {BusinessId} and restaurant {RestaurantId} with CorrelationId {CorrelationId}",
                context.Message.BusinessId, context.Message.RestaurantId, context.Message.CorrelationId);

            await context.Publish(new BusinessRestaurantCreatedFailureEvent
            {
                CorrelationId = context.Message.CorrelationId,
                BusinessId = context.Message.BusinessId,
                RestaurantId = context.Message.RestaurantId,
                Reason = ex.Message
            });
        }
    }
}