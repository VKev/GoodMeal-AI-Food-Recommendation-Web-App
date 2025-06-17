using SharedLibrary.Common.ResponseModel;
using SharedLibrary.Contracts.Authentication;

namespace Application.Services;

public interface IAuthenticationRepository
{
    Task<Result<GetUserStatusResponse>> GetUserStatusAsync(string identityId, CancellationToken cancellationToken = default);
    Task<Result<UserCommandResponse>> EnableUserAsync(string identityId, CancellationToken cancellationToken = default);
    Task<Result<UserCommandResponse>> DisableUserAsync(string identityId, CancellationToken cancellationToken = default);
    Task<Result<UserCommandResponse>> DeleteUserAsync(string identityId, CancellationToken cancellationToken = default);
    Task<Result<UpdateUserResponse>> UpdateUserAsync(string identityId, string? email = null, string? displayName = null, bool? emailVerified = null, CancellationToken cancellationToken = default);
    Task<Result<GetUserRolesResponse>> GetUserRolesAsync(string identityId, CancellationToken cancellationToken = default);
    Task<Result<RoleCommandResponse>> AddUserRoleAsync(string identityId, string roleName, CancellationToken cancellationToken = default);
    Task<Result<RoleCommandResponse>> RemoveUserRoleAsync(string identityId, string roleName, CancellationToken cancellationToken = default);
    Task<Result<SearchUsersResponse>> SearchUsersAsync(string? searchTerm = null, int pageSize = 50, string? pageToken = null, CancellationToken cancellationToken = default);
} 