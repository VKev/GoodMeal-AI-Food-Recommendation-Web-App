using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SubscriptionRepository : Repository<Subscription>, ISubscriptionRepository
{
    private readonly SubscriptionContext _context;

    public SubscriptionRepository(SubscriptionContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Subscription?> GetByIdAsync(Guid id)
    {
        return await _context.Subscriptions
            .Include(s => s.UserSubscriptions)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Subscription>> GetAllAsync()
    {
        return await _context.Subscriptions
            .Include(s => s.UserSubscriptions)
            .ToListAsync();
    }

    public async Task<IEnumerable<Subscription>> GetActiveSubscriptionsAsync()
    {
        return await _context.Subscriptions
            .Include(s => s.UserSubscriptions)
            .Where(s => s.IsActive && s.IsDisable != true)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Subscriptions.AnyAsync(s => s.Id == id);
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.Subscriptions.AnyAsync(s => s.Name == name);
    }
} 