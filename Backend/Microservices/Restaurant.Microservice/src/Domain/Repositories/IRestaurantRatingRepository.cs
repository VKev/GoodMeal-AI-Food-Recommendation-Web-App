using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories;

public interface IRestaurantRatingRepository : IRepository<RestaurantRating>
{
    public Task<IEnumerable<RestaurantRating?>> GetByRestaurantIdAsync(Guid restaurantId, CancellationToken cancellationToken);
    public Task<IEnumerable<RestaurantRating?>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}