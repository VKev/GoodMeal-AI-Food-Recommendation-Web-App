using Domain.Repositories;
using Infrastructure.Common;


namespace Infrastructure.Repositories;

public class MessageRestaurantRepository : Repository<MessageRestaurant>, IMessageRestaurantRepository
{
    public MessageRestaurantRepository(PromptDbContext context) : base(context)
    {
    }
}