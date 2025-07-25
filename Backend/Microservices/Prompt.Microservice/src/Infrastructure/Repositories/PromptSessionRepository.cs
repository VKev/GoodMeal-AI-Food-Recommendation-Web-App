using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

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

    public async Task SoftDeleteAllByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var entities = await _context.PromptSessions
            .Where(x => x.UserId == userId && !x.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedBy = userId;
        }
    }
}