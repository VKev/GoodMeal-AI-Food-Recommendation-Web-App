using Domain.Entities;
using SharedLibrary.Common;

namespace Domain.Repositories;

public interface IUserSubscriptionRepository : IRepository<UserSubscription>
{
    Task<UserSubscription?> GetByIdAsync(Guid id);
    Task<IEnumerable<UserSubscription>> GetByUserIdAsync(string userId);
    Task<UserSubscription?> GetActiveSubscriptionByUserIdAsync(string userId);
    Task<IEnumerable<UserSubscription>> GetBySubscriptionIdAsync(Guid subscriptionId);
    Task<bool> ExistsActiveSubscriptionForUserAsync(string userId);
} 