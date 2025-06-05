using Domain.Entities;

namespace Domain.Services;

public interface IUserService
{
    Task<UserRolesResponse?> GetUserRolesAsync(string identityId, CancellationToken cancellationToken = default);
}

