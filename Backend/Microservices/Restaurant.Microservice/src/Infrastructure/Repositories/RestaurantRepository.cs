using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Context;

namespace Infrastructure.Repositories;

public class RestaurantRepository: Repository<Restaurant>, IRestaurantRepository
{
    public RestaurantRepository(RestaurantFoodContext context) : base(context)
    {
    }
}