using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class MessageRepository : Repository<Message>, IMessageRepository

{
    public MessageRepository(PromptDbContext context) : base(context)
    {
    }

    public async Task SoftDeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Messages.FindAsync(new object[] { id }, cancellationToken);
        if (entity == null)
        {
            throw new KeyNotFoundException($"{nameof(PromptSession)} with id {id} not found.");
        }

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
    }

    public async Task<IEnumerable<Message>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Messages.Where(x => !x.IsDeleted)
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Message>> GetMessageActiveBySessionIdAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        var messages = await _context.Messages.Where(x => !x.IsDeleted && x.PromptSessionId == id)
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
        if (!messages.Any())
        {
            throw new KeyNotFoundException($"{nameof(Message)} with id {id} not found.");
        }

        return messages;
    }
}