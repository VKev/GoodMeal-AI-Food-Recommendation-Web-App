using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities;
using Infrastructure;

namespace Domain.Repositories;

public interface IMessageRepository : IRepository<Message>
{
    Task SoftDeleteByIdAsync(Guid id,string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Message>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Message>> GetMessageActiveBySessionIdAsync(Guid id, CancellationToken cancellationToken = default);
}