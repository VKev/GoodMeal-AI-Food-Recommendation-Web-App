using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories;

public interface IBusinessRepository : IRepository<Business>
{
    Task<Business?> GetByIdAsync(Guid id);
    Task<Business?> GetByOwnerIdAsync(string ownerId);
    Task<IEnumerable<Business>> GetAllAsync();
    Task<IEnumerable<Business>> GetByOwnerIdListAsync(IEnumerable<string> ownerIds);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsByOwnerIdAsync(string ownerId);
} 