using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories;

public interface IFoodRepository: IRepository<Food>
{
    public Task<IEnumerable<Food?>> GetByRestaurantIdAsync(Guid restaurantId, CancellationToken cancellationToken);
}