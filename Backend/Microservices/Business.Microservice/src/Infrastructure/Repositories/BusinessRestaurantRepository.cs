using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BusinessRestaurantRepository : Repository<BusinessRestaurant>, IBusinessRestaurantRepository
{
    public BusinessRestaurantRepository(BusinessRestaurantContext context) : base(context)
    {
    }

    public async Task<BusinessRestaurant?> GetByBusinessAndRestaurantIdAsync(Guid businessId, Guid restaurantId, CancellationToken cancellationToken)
    {
        return await _context.BusinessRestaurants
            .FirstOrDefaultAsync(br => br.BusinessId == businessId && br.RestaurantId == restaurantId, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<BusinessRestaurant>> GetByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken)
    {
        return await _context.BusinessRestaurants
            .Where(br => br.BusinessId == businessId)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<BusinessRestaurant>> GetByRestaurantIdAsync(Guid restaurantId, CancellationToken cancellationToken)
    {
        return await _context.BusinessRestaurants
            .Where(br => br.RestaurantId == restaurantId)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<bool> ExistsByBusinessAndRestaurantIdAsync(Guid businessId, Guid restaurantId, CancellationToken cancellationToken)
    {
        return await _context.BusinessRestaurants
            .AnyAsync(br => br.BusinessId == businessId && br.RestaurantId == restaurantId, cancellationToken: cancellationToken);
    }
} 