using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserSubscriptionRepository : Repository<UserSubscription>, IUserSubscriptionRepository
{
    private readonly SubscriptionContext _context;

    public UserSubscriptionRepository(SubscriptionContext context) : base(context)
    {
        _context = context;
    }

    public async Task<UserSubscription?> GetByIdAsync(Guid id)
    {
        return await _context.UserSubscriptions
            .Include(us => us.Subscription)
            .FirstOrDefaultAsync(us => us.Id == id);
    }

    public async Task<IEnumerable<UserSubscription>> GetByUserIdAsync(string userId)
    {
        return await _context.UserSubscriptions
            .Include(us => us.Subscription)
            .Where(us => us.UserId == userId)
            .ToListAsync();
    }

    public async Task<UserSubscription?> GetActiveSubscriptionByUserIdAsync(string userId)
    {
        return await _context.UserSubscriptions
            .Include(us => us.Subscription)
            .Where(us => us.UserId == userId && us.IsActive && us.EndDate > DateTime.Now)
            .OrderByDescending(us => us.EndDate)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<UserSubscription>> GetBySubscriptionIdAsync(Guid subscriptionId)
    {
        return await _context.UserSubscriptions
            .Include(us => us.Subscription)
            .Where(us => us.SubscriptionId == subscriptionId)
            .ToListAsync();
    }

    public async Task<bool> ExistsActiveSubscriptionForUserAsync(string userId)
    {
        return await _context.UserSubscriptions
            .AnyAsync(us => us.UserId == userId && us.IsActive && us.EndDate > DateTime.Now);
    }
} 