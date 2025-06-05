using Domain.Entities;
using SharedLibrary.Common;

namespace Domain.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByIdentityIdAsync(string identityId, CancellationToken cancellationToken);
        Task<User?> GetByIdWithRolesAsync(Guid userId, CancellationToken cancellationToken);
        Task EditName(Guid userId, string name, CancellationToken cancellationToken);
        Task AddRole(Guid userId, Guid roleId, CancellationToken cancellationToken);
        Task RemoveRole(Guid userId, Guid roleId, CancellationToken cancellationToken);
        Task RemoveUserAsync(Guid userId, CancellationToken cancellationToken);
    }
}