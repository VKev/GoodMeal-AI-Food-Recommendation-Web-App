using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Common;


namespace Infrastructure.Repositories;

public class PromptSessionRepository : Repository<PromptSession>, IPromptSessionRepository
{
    public PromptSessionRepository(PromptDbContext context) : base(context)
    {
    }

    public async Task SoftDeleteByIdAsync(Guid id, string userId, CancellationToken cancellationToken = default)
    {
        var entity = await _context.PromptSessions.FindAsync(new object[] { id }, cancellationToken);
        if (entity == null)
        {
            throw new KeyNotFoundException($"{nameof(PromptSession)} with id {id} not found.");
        }
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.DeletedBy = userId;
    }
}