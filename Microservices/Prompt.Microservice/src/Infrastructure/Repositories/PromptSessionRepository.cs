using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Context;

namespace Infrastructure.Repositories;

public class PromptSessionRepository : Repository<PromptSession>,IPromptSessionRepository
{
    public PromptSessionRepository(PromptDbContext context) : base(context)
    {
    }
}