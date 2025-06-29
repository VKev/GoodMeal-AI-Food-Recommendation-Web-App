using System.Linq.Expressions;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class FoodImageRepository : Repository<FoodImage>, IFoodImageRepository
{
    public FoodImageRepository(PromptDbContext context) : base(context)
    {
    }

    public async Task<List<FoodImage>> GetByNamesAsync(List<string> foodNames, CancellationToken cancellationToken)
    {
        var lowerFoodNames = foodNames.Select(n => n.ToLower()).ToList();

        return await _context.FoodImages
            .Where(x => lowerFoodNames.Contains(x.FoodName.ToLower()))
            .ToListAsync(cancellationToken);
    }

    public async Task<FoodImage?> GetByNameAsync(string foodName, CancellationToken cancellationToken)
    {
        return await _context.FoodImages
            .FirstOrDefaultAsync(x => x.FoodName == foodName, cancellationToken);
    }

    
    public async Task<bool> AnyAsync(Expression<Func<FoodImage, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _context.FoodImages.AnyAsync(predicate, cancellationToken);
    }
}