using Domain.Entities;
using SharedLibrary.Common;

namespace Domain.Repositories;

public interface ISubscriptionPaymentStatusRepository : IRepository<SubscriptionPaymentStatus>
{
    Task<SubscriptionPaymentStatus?> GetByCorrelationIdAsync(Guid correlationId, CancellationToken cancellationToken = default);
    Task<SubscriptionPaymentStatus?> GetByOrderIdAsync(string orderId, CancellationToken cancellationToken = default);
} 