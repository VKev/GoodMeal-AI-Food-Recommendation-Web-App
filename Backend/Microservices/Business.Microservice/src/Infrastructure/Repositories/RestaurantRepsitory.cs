using Domain.Entities;
using Domain.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;
using SharedLibrary.Contracts.Restaurant;

namespace Infrastructure.Services;

public class RestaurantRepsitory : IRestaurantRepository
{
    private readonly IBus _bus;
    private readonly ILogger<RestaurantRepsitory> _logger;

    public RestaurantRepsitory(IBus bus, ILogger<RestaurantRepsitory> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task<RestaurantInfo> GetRestaurantByIdAsync(Guid restaurantId, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _bus.CreateRequestClient<GetRestaurantByIdRequest>();
            var response = await client.GetResponse<GetRestaurantByIdResponse>(new GetRestaurantByIdRequest { Id = restaurantId }, cancellationToken);
            
            if (!response.Message.IsSuccess)
            {
                throw new Exception("Failed to get restaurant");
            }

            var restaurantInfo = new RestaurantInfo(
                response.Message.Restaurant.Id,
                response.Message.Restaurant.Name,
                response.Message.Restaurant.Description,
                response.Message.Restaurant.Address,
                response.Message.Restaurant.Phone,
                response.Message.Restaurant.Email,
                response.Message.Restaurant.IsActive,
                response.Message.Restaurant.CreatedAt
            );

            return restaurantInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting restaurant {RestaurantId}", restaurantId);
            throw  new Exception("Failed to get restaurant");
        }
    }

    public async Task<IEnumerable<RestaurantInfo>> GetRestaurantsByIdsAsync(IEnumerable<Guid> restaurantIds, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _bus.CreateRequestClient<GetRestaurantsByIdsRequest>();
            var response = await client.GetResponse<GetRestaurantsByIdsResponse>(new GetRestaurantsByIdsRequest { Ids = restaurantIds.ToList() }, cancellationToken);
            
            if (!response.Message.IsSuccess)
            {
                throw new Exception("Failed to get restaurants");
            }

            var restaurantInfos = response.Message.Restaurants.Select(r => new RestaurantInfo(
                r.Id,
                r.Name,
                r.Description,
                r.Address,
                r.Phone,
                r.Email,
                r.IsActive,
                r.CreatedAt
            ));

            return restaurantInfos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting restaurants {RestaurantIds}", string.Join(",", restaurantIds));
            throw  new Exception("Failed to get restaurant");
        }
    }
} 