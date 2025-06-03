using Domain.Entities;
using SharedLibrary.Common;

namespace Domain.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task EditName(string userId, string name, CancellationToken cancellationToken);
        Task AddRole(string userId, string roleId, CancellationToken cancellationToken);
        Task RemoveRole(string userId, string roleId, CancellationToken cancellationToken);
        Task RemoveUserAsync(string userId, CancellationToken cancellationToken);
    }
}