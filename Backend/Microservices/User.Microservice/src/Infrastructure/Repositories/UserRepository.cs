using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Common;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(MyDbContext context) : base(context)
        {
        }

        public async Task<User> GetByIdentityIdAsync(string identityId, CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.IdentityId == identityId, cancellationToken);
            if (user == null)
                throw new NullReferenceException("User not found");

            return user;
        }

        public async Task<User?> GetByIdWithRolesAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Set<User>()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
        }

        public async Task EditName(Guid userId, string name, CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>().FindAsync(userId, cancellationToken);
            if (user == null)
                throw new NullReferenceException("User not found");

            user.Name = name;
            user.UpdateAt = DateTime.UtcNow;
        }

        public async Task AddRole(Guid userId, Guid roleId, CancellationToken cancellationToken)
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
                AssignedAt = DateTime.UtcNow,
            }, cancellationToken);
        }

        public async Task RemoveRole(Guid userId, Guid roleId, CancellationToken cancellationToken)
        {
            var userRole = await _context.Set<UserRole>()
                .FindAsync(new object[] { userId, roleId }, cancellationToken);

            if (userRole == null)
                throw new NullReferenceException("User role association not found");

            _context.Set<UserRole>().Remove(userRole);
        }

        public async Task RemoveUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>().FindAsync(userId);
            if (user == null)
                throw new NullReferenceException("User not found");

            user.IsDeleted = true;
        }
    }
}