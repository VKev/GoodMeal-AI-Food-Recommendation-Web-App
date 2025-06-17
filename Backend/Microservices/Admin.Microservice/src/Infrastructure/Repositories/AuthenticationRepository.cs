using Application.Services;
using MassTransit;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.ResponseModel;
using SharedLibrary.Contracts.Authentication;

namespace Infrastructure.Repositories;

public class AuthenticationRepository : IAuthenticationRepository
{
    private readonly IBus _bus;
    private readonly ILogger<AuthenticationRepository> _logger;

    public AuthenticationRepository(IBus bus, ILogger<AuthenticationRepository> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task<Result<GetUserStatusResponse>> GetUserStatusAsync(string identityId, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _bus.CreateRequestClient<GetUserStatusRequest>();
            var response = await client.GetResponse<GetUserStatusResponse>(new GetUserStatusRequest { IdentityId = identityId }, cancellationToken);
            
            return Result.Success(response.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user status for {IdentityId}", identityId);
            return Result.Failure<GetUserStatusResponse>(new Error("AuthService.GetUserStatus", "Failed to get user status"));
        }
    }

    public async Task<Result<UserCommandResponse>> EnableUserAsync(string identityId, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _bus.CreateRequestClient<EnableUserRequest>();
            var response = await client.GetResponse<UserCommandResponse>(new EnableUserRequest { IdentityId = identityId }, cancellationToken);
            
            return Result.Success(response.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enabling user {IdentityId}", identityId);
            return Result.Failure<UserCommandResponse>(new Error("AuthService.EnableUser", "Failed to enable user"));
        }
    }

    public async Task<Result<UserCommandResponse>> DisableUserAsync(string identityId, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _bus.CreateRequestClient<DisableUserRequest>();
            var response = await client.GetResponse<UserCommandResponse>(new DisableUserRequest { IdentityId = identityId }, cancellationToken);
            
            return Result.Success(response.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disabling user {IdentityId}", identityId);
            return Result.Failure<UserCommandResponse>(new Error("AuthService.DisableUser", "Failed to disable user"));
        }
    }

    public async Task<Result<UserCommandResponse>> DeleteUserAsync(string identityId, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _bus.CreateRequestClient<DeleteUserRequest>();
            var response = await client.GetResponse<UserCommandResponse>(new DeleteUserRequest { IdentityId = identityId }, cancellationToken);
            
            return Result.Success(response.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {IdentityId}", identityId);
            return Result.Failure<UserCommandResponse>(new Error("AuthService.DeleteUser", "Failed to delete user"));
        }
    }

    public async Task<Result<UpdateUserResponse>> UpdateUserAsync(string identityId, string? email = null, string? displayName = null, bool? emailVerified = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _bus.CreateRequestClient<UpdateUserRequest>();
            var response = await client.GetResponse<UpdateUserResponse>(new UpdateUserRequest 
            { 
                IdentityId = identityId,
                Email = email,
                DisplayName = displayName,
                EmailVerified = emailVerified
            }, cancellationToken);
            
            return Result.Success(response.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {IdentityId}", identityId);
            return Result.Failure<UpdateUserResponse>(new Error("AuthService.UpdateUser", "Failed to update user"));
        }
    }

    public async Task<Result<GetUserRolesResponse>> GetUserRolesAsync(string identityId, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _bus.CreateRequestClient<GetUserRolesRequest>();
            var response = await client.GetResponse<GetUserRolesResponse>(new GetUserRolesRequest { IdentityId = identityId }, cancellationToken);
            
            return Result.Success(response.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user roles for {IdentityId}", identityId);
            return Result.Failure<GetUserRolesResponse>(new Error("AuthService.GetUserRoles", "Failed to get user roles"));
        }
    }

    public async Task<Result<RoleCommandResponse>> AddUserRoleAsync(string identityId, string roleName, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _bus.CreateRequestClient<AddUserRoleRequest>();
            var response = await client.GetResponse<RoleCommandResponse>(new AddUserRoleRequest 
            { 
                IdentityId = identityId,
                RoleName = roleName
            }, cancellationToken);
            
            return Result.Success(response.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding role {RoleName} to user {IdentityId}", roleName, identityId);
            return Result.Failure<RoleCommandResponse>(new Error("AuthService.AddUserRole", "Failed to add user role"));
        }
    }

    public async Task<Result<RoleCommandResponse>> RemoveUserRoleAsync(string identityId, string roleName, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _bus.CreateRequestClient<RemoveUserRoleRequest>();
            var response = await client.GetResponse<RoleCommandResponse>(new RemoveUserRoleRequest 
            { 
                IdentityId = identityId,
                RoleName = roleName
            }, cancellationToken);
            
            return Result.Success(response.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role {RoleName} from user {IdentityId}", roleName, identityId);
            return Result.Failure<RoleCommandResponse>(new Error("AuthService.RemoveUserRole", "Failed to remove user role"));
        }
    }

    public async Task<Result<SearchUsersResponse>> SearchUsersAsync(string? searchTerm = null, int pageSize = 50, string? pageToken = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _bus.CreateRequestClient<SearchUsersRequest>();
            var response = await client.GetResponse<SearchUsersResponse>(new SearchUsersRequest 
            { 
                SearchTerm = searchTerm,
                PageSize = pageSize,
                PageToken = pageToken
            }, cancellationToken);
            
            return Result.Success(response.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users with term {SearchTerm}", searchTerm);
            return Result.Failure<SearchUsersResponse>(new Error("AuthService.SearchUsers", "Failed to search users"));
        }
    }
} 