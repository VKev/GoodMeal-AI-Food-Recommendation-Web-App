using Infrastructure.Common;
using Infrastructure.Context;
using SharedLibrary.Common;

namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RestaurantFoodContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(RestaurantFoodContext context)
        {
            _context = context;
        }

        public IRepository<T> Repository<T>() where T : class
        {
            if (!_repositories.ContainsKey(typeof(T)))
            {
                _repositories[typeof(T)] = new Repository<T>(_context);
            }

            return (IRepository<T>)_repositories[typeof(T)];
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

}