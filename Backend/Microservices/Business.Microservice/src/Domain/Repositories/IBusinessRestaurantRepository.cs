using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories;

public interface IBusinessRestaurantRepository : IRepository<BusinessRestaurant>
{
    Task<BusinessRestaurant?> GetByBusinessAndRestaurantIdAsync(Guid businessId, Guid restaurantId);
    Task<IEnumerable<BusinessRestaurant>> GetByBusinessIdAsync(Guid businessId);
    Task<IEnumerable<BusinessRestaurant>> GetByRestaurantIdAsync(Guid restaurantId);
    Task<bool> ExistsByBusinessAndRestaurantIdAsync(Guid businessId, Guid restaurantId);
} 