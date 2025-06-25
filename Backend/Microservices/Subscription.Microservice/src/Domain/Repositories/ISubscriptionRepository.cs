using Domain.Entities;
using SharedLibrary.Common;

namespace Domain.Repositories;

public interface ISubscriptionRepository : IRepository<Subscription>
{
    Task<Subscription?> GetByIdAsync(Guid id);
    Task<IEnumerable<Subscription>> GetAllAsync();
    Task<IEnumerable<Subscription>> GetActiveSubscriptionsAsync();
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsByNameAsync(string name);
} 