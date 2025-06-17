using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Common;
using Infrastructure;

namespace Domain.Repositories;

public interface IPromptSessionRepository : IRepository<PromptSession>
{
    public Task SoftDeleteByIdAsync(Guid id, CancellationToken cancellationToken = default);
}