using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Context;

namespace Infrastructure.Repositories;

public class MessageRepository : Repository<Message>,IMessageRepository

{
    public MessageRepository(PromptDbContext context) : base(context)
    {
    }
}