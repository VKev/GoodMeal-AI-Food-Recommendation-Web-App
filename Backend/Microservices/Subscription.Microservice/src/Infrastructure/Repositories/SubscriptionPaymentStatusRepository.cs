using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SubscriptionPaymentStatusRepository : Repository<SubscriptionPaymentStatus>, ISubscriptionPaymentStatusRepository
{
    public SubscriptionPaymentStatusRepository(SubscriptionContext context) : base(context)
    {
    }

    public async Task<SubscriptionPaymentStatus?> GetByCorrelationIdAsync(Guid correlationId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<SubscriptionPaymentStatus>()
            .FirstOrDefaultAsync(x => x.CorrelationId == correlationId, cancellationToken);
    }

    public async Task<SubscriptionPaymentStatus?> GetByOrderIdAsync(string orderId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<SubscriptionPaymentStatus>()
            .FirstOrDefaultAsync(x => x.OrderId == orderId, cancellationToken);
    }
} 