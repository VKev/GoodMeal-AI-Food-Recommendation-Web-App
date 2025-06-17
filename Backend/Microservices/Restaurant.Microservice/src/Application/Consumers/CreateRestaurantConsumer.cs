using MassTransit;
using Microsoft.Extensions.Logging;
using SharedLibrary.Contracts.RestaurantCreating;
using Domain.Repositories;
using Domain.Entities;
using SharedLibrary.Common;

namespace Application.Consumers;

public class CreateRestaurantConsumer : IConsumer<CreateRestaurantEvent>
{
    private readonly ILogger<CreateRestaurantConsumer> _logger;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRestaurantConsumer(
        ILogger<CreateRestaurantConsumer> logger,
        IRestaurantRepository restaurantRepository,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _restaurantRepository = restaurantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<CreateRestaurantEvent> context)
    {
        _logger.LogInformation(
            "CreateRestaurantConsumer: Received CreateRestaurantEvent for restaurant {RestaurantId} with CorrelationId {CorrelationId}",
            context.Message.RestaurantId, context.Message.CorrelationId);

        try
        {
            try
            {
                var existingRestaurant =
                    await _restaurantRepository.GetByIdAsync(context.Message.RestaurantId, context.CancellationToken);
                await context.Publish(new RestaurantCreatedFailureEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    RestaurantId = context.Message.RestaurantId,
                    Reason = "Restaurant with this ID already exists"
                });
                return;
            }
            catch (KeyNotFoundException)
            {
            }

            var restaurant = new Restaurant
            {
                Id = context.Message.RestaurantId,
                Name = context.Message.Name,
                Address = context.Message.Address,
                Phone = context.Message.Phone,
                CreatedBy = context.Message.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDisable = false
            };

            await _restaurantRepository.AddAsync(restaurant, context.CancellationToken);
            await _unitOfWork.SaveChangesAsync(context.CancellationToken);

            await context.Publish(new RestaurantCreatedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                RestaurantId = context.Message.RestaurantId,
                Name = context.Message.Name
            });

            _logger.LogInformation("Restaurant {RestaurantId} created successfully with CorrelationId {CorrelationId}",
                context.Message.RestaurantId, context.Message.CorrelationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating restaurant {RestaurantId} with CorrelationId {CorrelationId}",
                context.Message.RestaurantId, context.Message.CorrelationId);

            await context.Publish(new RestaurantCreatedFailureEvent
            {
                CorrelationId = context.Message.CorrelationId,
                RestaurantId = context.Message.RestaurantId,
                Reason = ex.Message
            });
        }
    }
}