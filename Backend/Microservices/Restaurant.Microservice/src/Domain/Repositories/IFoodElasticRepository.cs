using Domain.Entities;

namespace Domain.Repositories;

public interface IFoodElasticRepository
{
    Task<IEnumerable<Guid>> SearchRestaurantIdsByFoodNameAsync(string keyword, CancellationToken cancellationToken = default);
    Task CreateIndexIfNotExistsAsync(string indexName, CancellationToken cancellationToken = default);
    Task<bool> AddOrUpdate (Food food, CancellationToken cancellationToken = default);
    Task<bool> AddOrUpdateBulk (IEnumerable<Food> foods, CancellationToken cancellationToken = default);
    Task<Food> Get(string key,  CancellationToken cancellationToken = default);
    Task<IEnumerable<Food>> GetAll(CancellationToken cancellationToken = default);
    Task<bool> Remove(string key, CancellationToken cancellationToken = default);
    Task<long?> RemoveAll(CancellationToken cancellationToken = default);
}