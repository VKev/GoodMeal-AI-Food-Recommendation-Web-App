using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Context;

namespace Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(MyDbContext context) : base(context)
        {
        }

        public async Task EditName(string userId, string name, CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>().FindAsync(userId, cancellationToken);
            if (user == null)
                throw new NullReferenceException("User not found");

            user.Name = name;
        }

        public async Task AddRole(string userId, string roleId, CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>().FindAsync(userId);
            if (user == null)
                throw new NullReferenceException("User not found");

            var role = await _context.Set<Role>().FindAsync(roleId);
            if (role == null)
                throw new NullReferenceException("Role not found");

            await _context.Set<UserRole>().AddAsync(new UserRole()
            {
                UserId = user.UserId,
                RoleId = role.RoleId,
            }, cancellationToken);
        }

        public async Task RemoveRole(string userId, string roleId, CancellationToken cancellationToken)
        {
            var userRole = await _context.Set<UserRole>()
                .FindAsync(new object[] { userId, roleId }, cancellationToken);

            if (userRole == null)
                throw new NullReferenceException("User role association not found");

            _context.Set<UserRole>().Remove(userRole);
        }

        public async Task RemoveUserAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>().FindAsync(userId);
            if (user == null)
                throw new NullReferenceException("User not found");
            
            user.IdentityId = null;
        }
    }
}