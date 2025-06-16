using MassTransit;
using Microsoft.Extensions.Logging;
using SharedLibrary.Contracts.Restaurant;
using Domain.Repositories;

namespace Application.Consumers;

public class GetRestaurantsByIdsConsumer : IConsumer<GetRestaurantsByIdsRequest>
{
    private readonly ILogger<GetRestaurantsByIdsConsumer> _logger;
    private readonly IRestaurantRepository _restaurantRepository;

    public GetRestaurantsByIdsConsumer(ILogger<GetRestaurantsByIdsConsumer> logger, IRestaurantRepository restaurantRepository)
    {
        _logger = logger;
        _restaurantRepository = restaurantRepository;
    }

    public async Task Consume(ConsumeContext<GetRestaurantsByIdsRequest> context)
    {
        try
        {
            var restaurants = new List<Domain.Entities.Restaurant>();
            
            foreach (var id in context.Message.Ids)
            {
                var restaurant = await _restaurantRepository.GetByIdAsync(id, context.CancellationToken);
                restaurants.Add(restaurant);
            }

            var restaurantDtos = restaurants.Select(r => new RestaurantDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = null,
                Address = r.Address,
                Phone = r.Phone,
                Email = null,
                IsActive = !r.IsDisable ?? true,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            }).ToList();

            var response = new GetRestaurantsByIdsResponse
            {
                IsSuccess = true,
                Message = $"Found {restaurantDtos.Count} restaurants",
                Restaurants = restaurantDtos
            };

            await context.RespondAsync(response);
        }
        catch (Exception ex)
        {
            var errorResponse = new GetRestaurantsByIdsResponse
            {
                IsSuccess = false,
                Message = "Internal server error occurred while fetching restaurants",
                Restaurants = new List<RestaurantDto>()
            };

            await context.RespondAsync(errorResponse);
        }
    }
} 