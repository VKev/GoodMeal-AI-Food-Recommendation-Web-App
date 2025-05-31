using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Context;

namespace Infrastructure.Repositories;

public class MessageRestaurantRepository : Repository<MessageRestaurant>, IMessageRestaurantRepository
{
    public MessageRestaurantRepository(PromptDbContext context) : base(context)
    {
    }
}