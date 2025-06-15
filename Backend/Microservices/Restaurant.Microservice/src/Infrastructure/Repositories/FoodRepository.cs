using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Context;

namespace Infrastructure.Repositories;

public class FoodRepository: Repository<Food>, IFoodRepository
{
    public FoodRepository(RestaurantFoodContext context) : base(context)
    {
    }
}