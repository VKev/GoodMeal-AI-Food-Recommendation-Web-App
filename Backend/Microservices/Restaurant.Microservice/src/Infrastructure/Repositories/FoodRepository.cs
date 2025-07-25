using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class FoodRepository: Repository<Food>, IFoodRepository
{
    private readonly RestaurantFoodContext _context;

    public FoodRepository(RestaurantFoodContext context) : base(context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Food?>> GetByRestaurantIdAsync(Guid restaurantId, CancellationToken cancellationToken = default)
    {
        return await _context.Foods
            .Where(r => r.RestaurantId == restaurantId && (r.IsDisable == false || r.IsDisable == null))
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}