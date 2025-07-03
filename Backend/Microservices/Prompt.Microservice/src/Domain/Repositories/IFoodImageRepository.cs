using System.Linq.Expressions;
using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories;

public interface IFoodImageRepository : IRepository<FoodImage>
{
    Task<List<FoodImage>> GetByNamesAsync(List<string> foodNames, CancellationToken cancellationToken);
    
    Task<FoodImage?> GetByNameAsync(string foodName, CancellationToken cancellationToken);
    Task<bool> AnyAsync(Expression<Func<FoodImage, bool>> predicate, CancellationToken cancellationToken);
}