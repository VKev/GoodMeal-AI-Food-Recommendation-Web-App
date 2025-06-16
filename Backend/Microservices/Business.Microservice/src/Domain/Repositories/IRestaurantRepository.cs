using Domain.Entities;

namespace Domain.Repositories;

public interface IRestaurantRepository
{
    Task<RestaurantInfo> GetRestaurantByIdAsync(Guid restaurantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RestaurantInfo>> GetRestaurantsByIdsAsync(IEnumerable<Guid> restaurantIds, CancellationToken cancellationToken = default);
} 