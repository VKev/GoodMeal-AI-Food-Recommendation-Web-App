using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities;
using Infrastructure;

namespace Domain.Repositories;

public interface IPromptSessionRepository : IRepository<PromptSession>
{
    public Task SoftDeleteByIdAsync(Guid id, string userId, CancellationToken cancellationToken = default);
    public Task SoftDeleteAllByUserIdAsync(string userId, CancellationToken cancellationToken = default);
}