using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RestaurantRatingRepository : Repository<RestaurantRating>, IRestaurantRatingRepository
{
    private readonly RestaurantFoodContext _context;
    public RestaurantRatingRepository(RestaurantFoodContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RestaurantRating?>> GetByRestaurantIdAsync(Guid restaurantId, CancellationToken cancellationToken = default)
    {
        return await _context.RestaurantRatings
            .Where(r => r.RestaurantId == restaurantId && (r.IsDisable == false || r.IsDisable == null))
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RestaurantRating?>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.RestaurantRatings
            .Where(r => r.UserId == userId && (r.IsDisable == false || r.IsDisable == null))
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}