using FirebaseAdmin;
using FirebaseAdmin.Auth;
using MassTransit;
using Microsoft.Extensions.Logging;
using SharedLibrary.Contracts.Authentication;
using Newtonsoft.Json.Linq;

namespace Application.Consumers;

public class GetUserRolesConsumer : IConsumer<GetUserRolesRequest>
{
    private readonly ILogger<GetUserRolesConsumer> _logger;

    public GetUserRolesConsumer(ILogger<GetUserRolesConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<GetUserRolesRequest> context)
    {
        try
        {
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(context.Message.IdentityId, context.CancellationToken);

            var roles = new List<string>();
            if (userRecord.CustomClaims != null && userRecord.CustomClaims.TryGetValue("roles", out var rolesObj))
            {
                if (rolesObj is string[] roleArray)
                {
                    roles = roleArray.ToList();
                }
                else if (rolesObj is JArray jArray)
                {
                    roles = jArray.ToObject<string[]>()?.ToList() ?? new List<string>();
                }
                else if (rolesObj is List<string> roleList)
                {
                    roles = roleList;
                }
                else if (rolesObj is string singleRole)
                {
                    roles = new List<string> { singleRole };
                }
            }

            var response = new GetUserRolesResponse
            {
                IdentityId = userRecord.Uid,
                Email = userRecord.Email ?? "",
                Name = userRecord.DisplayName ?? "",
                Roles = roles
            };

            await context.RespondAsync(response);
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogError(ex, "Firebase error getting user roles for {IdentityId}", context.Message.IdentityId);
            
            await context.RespondAsync(new GetUserRolesResponse 
            { 
                IdentityId = context.Message.IdentityId,
                Email = "",
                Name = "",
                Roles = new List<string>()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting user roles for {IdentityId}", context.Message.IdentityId);
            throw;
        }
    }
}

public class AddUserRoleConsumer : IConsumer<AddUserRoleRequest>
{
    private readonly ILogger<AddUserRoleConsumer> _logger;

    public AddUserRoleConsumer(ILogger<AddUserRoleConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<AddUserRoleRequest> context)
    {
        try
        {
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(context.Message.IdentityId, context.CancellationToken);

            var currentRoles = new List<string>();
            if (userRecord.CustomClaims != null && userRecord.CustomClaims.TryGetValue("roles", out var rolesObj))
            {
                if (rolesObj is string[] roles)
                {
                    currentRoles = roles.ToList();
                }
            }

            if (currentRoles.Contains(context.Message.RoleName))
            {
                await context.RespondAsync(new RoleCommandResponse 
                { 
                    IsSuccess = false, 
                    Message = "User already has this role",
                    ErrorCode = "RoleAlreadyExists"
                });
                return;
            }

            currentRoles.Add(context.Message.RoleName);

            var customClaims = new Dictionary<string, object>();
            if (userRecord.CustomClaims != null)
            {
                foreach (var claim in userRecord.CustomClaims)
                {
                    customClaims[claim.Key] = claim.Value;
                }
            }

            customClaims["roles"] = currentRoles.ToArray();
            customClaims["updated_at"] = DateTime.UtcNow.ToString("O");

            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(context.Message.IdentityId, customClaims, context.CancellationToken);

            await FirebaseAuth.DefaultInstance.RevokeRefreshTokensAsync(context.Message.IdentityId, context.CancellationToken);

            _logger.LogInformation("Successfully added role {RoleName} to user {IdentityId}", context.Message.RoleName, context.Message.IdentityId);

            await context.RespondAsync(new RoleCommandResponse 
            { 
                IsSuccess = true, 
                Message = "Role added successfully."
            });
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogError(ex, "Firebase error adding role {RoleName} to user {IdentityId}", context.Message.RoleName, context.Message.IdentityId);
            
            await context.RespondAsync(new RoleCommandResponse 
            { 
                IsSuccess = false, 
                Message = ex.ErrorCode == ErrorCode.NotFound ? "User not found in Firebase" : "Failed to add role to user",
                ErrorCode = ex.ErrorCode.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error adding role {RoleName} to user {IdentityId}", context.Message.RoleName, context.Message.IdentityId);
            throw;
        }
    }
}

public class RemoveUserRoleConsumer : IConsumer<RemoveUserRoleRequest>
{
    private readonly ILogger<RemoveUserRoleConsumer> _logger;

    public RemoveUserRoleConsumer(ILogger<RemoveUserRoleConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<RemoveUserRoleRequest> context)
    {
        try
        {
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(context.Message.IdentityId, context.CancellationToken);
            
            var currentRoles = new List<string>();
            if (userRecord.CustomClaims != null && userRecord.CustomClaims.TryGetValue("roles", out var rolesObj))
            {
                if (rolesObj is string[] roles)
                {
                    currentRoles = roles.ToList();
                }
            }

            if (!currentRoles.Contains(context.Message.RoleName))
            {
                await context.RespondAsync(new RoleCommandResponse 
                { 
                    IsSuccess = false, 
                    Message = "User does not have this role",
                    ErrorCode = "RoleNotFound"
                });
                return;
            }

            currentRoles.Remove(context.Message.RoleName);

            var customClaims = new Dictionary<string, object>();
            if (userRecord.CustomClaims != null)
            {
                foreach (var claim in userRecord.CustomClaims)
                {
                    customClaims[claim.Key] = claim.Value;
                }
            }

            customClaims["roles"] = currentRoles.ToArray();
            customClaims["updated_at"] = DateTime.UtcNow.ToString("O");

            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(context.Message.IdentityId, customClaims, context.CancellationToken);
            await FirebaseAuth.DefaultInstance.RevokeRefreshTokensAsync(context.Message.IdentityId, context.CancellationToken);

            _logger.LogInformation("Successfully removed role {RoleName} from user {IdentityId}", context.Message.RoleName, context.Message.IdentityId);

            await context.RespondAsync(new RoleCommandResponse 
            { 
                IsSuccess = true, 
                Message = "Role removed from user."
            });
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogError(ex, "Firebase error removing role {RoleName} from user {IdentityId}", context.Message.RoleName, context.Message.IdentityId);
            
            await context.RespondAsync(new RoleCommandResponse 
            { 
                IsSuccess = false, 
                Message = ex.ErrorCode == ErrorCode.NotFound ? "User not found in Firebase" : "Failed to remove role from user",
                ErrorCode = ex.ErrorCode.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error removing role {RoleName} from user {IdentityId}", context.Message.RoleName, context.Message.IdentityId);
            throw;
        }
    }
} 