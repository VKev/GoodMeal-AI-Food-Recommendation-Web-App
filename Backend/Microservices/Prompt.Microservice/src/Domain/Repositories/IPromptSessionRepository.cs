using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories;

public interface IPromptSessionRepository : IRepository<PromptSession>
{
    public Task SoftDeleteByIdAsync(Guid id, CancellationToken cancellationToken = default);
}