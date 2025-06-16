using MassTransit;
using Microsoft.Extensions.Logging;
using SharedLibrary.Contracts.Restaurant;
using Domain.Repositories;

namespace Application.Consumers;

public class GetRestaurantByIdConsumer : IConsumer<GetRestaurantByIdRequest>
{
    private readonly ILogger<GetRestaurantByIdConsumer> _logger;
    private readonly IRestaurantRepository _restaurantRepository;

    public GetRestaurantByIdConsumer(ILogger<GetRestaurantByIdConsumer> logger, IRestaurantRepository restaurantRepository)
    {
        _logger = logger;
        _restaurantRepository = restaurantRepository;
    }

    public async Task Consume(ConsumeContext<GetRestaurantByIdRequest> context)
    {
        try
        {

            var restaurant = await _restaurantRepository.GetByIdAsync(context.Message.Id, context.CancellationToken);

            var restaurantDto = new RestaurantDto
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Description = null,
                Address = restaurant.Address,
                Phone = restaurant.Phone,
                Email = null,
                IsActive = !restaurant.IsDisable ?? true,
                CreatedAt = restaurant.CreatedAt,
                UpdatedAt = restaurant.UpdatedAt
            };

            var response = new GetRestaurantByIdResponse
            {
                IsSuccess = true,
                Message = "Restaurant found successfully",
                Restaurant = restaurantDto
            };

            await context.RespondAsync(response);
            
            _logger.LogInformation("Successfully processed GetRestaurantByIdRequest for restaurant {RestaurantId}", context.Message.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing GetRestaurantByIdRequest for restaurant {RestaurantId}", context.Message.Id);
            
            var errorResponse = new GetRestaurantByIdResponse
            {
                IsSuccess = false,
                Message = "Internal server error occurred while fetching restaurant",
                Restaurant = new RestaurantDto()
            };

            await context.RespondAsync(errorResponse);
        }
    }
} 