using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories;

public interface IBusinessRestaurantRepository : IRepository<BusinessRestaurant>
{
    Task<BusinessRestaurant?> GetByBusinessAndRestaurantIdAsync(Guid businessId, Guid restaurantId,
        CancellationToken cancellationToken);

    Task<IEnumerable<BusinessRestaurant>> GetByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken);

    Task<IEnumerable<BusinessRestaurant>>
        GetByRestaurantIdAsync(Guid restaurantId, CancellationToken cancellationToken);

    Task<bool> ExistsByBusinessAndRestaurantIdAsync(Guid businessId, Guid restaurantId,
        CancellationToken cancellationToken);
}