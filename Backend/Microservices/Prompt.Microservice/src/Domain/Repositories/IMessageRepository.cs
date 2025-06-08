using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories;

public interface IMessageRepository : IRepository<Message>
{
    Task SoftDeleteByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Message>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Message>> GetMessageActiveBySessionIdAsync(Guid id, CancellationToken cancellationToken = default);
}