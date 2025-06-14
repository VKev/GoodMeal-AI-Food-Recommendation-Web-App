using SharedLibrary.Common.Messaging;
using FluentValidation;
using SharedLibrary.Common.ResponseModel;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;
using FirebaseAdmin;

namespace Application.Auths.Commands.RemoveUserRoleCommand;

public sealed record RemoveUserRoleCommand(string IdentityId, string RoleName) : ICommand;

internal sealed class RemoveUserRoleCommandHandler : ICommandHandler<RemoveUserRoleCommand>
{
    private readonly ILogger<RemoveUserRoleCommandHandler> _logger;

    public RemoveUserRoleCommandHandler(ILogger<RemoveUserRoleCommandHandler> logger)
    {
        _logger = logger;
    }

    public async Task<Result> Handle(RemoveUserRoleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(request.IdentityId, cancellationToken);
            
            var currentRoles = new List<string>();
            if (userRecord.CustomClaims != null && userRecord.CustomClaims.TryGetValue("roles", out var rolesObj))
            {
                if (rolesObj is string[] roles)
                {
                    currentRoles = roles.ToList();
                }
            }

            if (!currentRoles.Contains(request.RoleName))
            {
                return Result.Failure(new Error("RoleNotFound", "User does not have this role"));
            }

            currentRoles.Remove(request.RoleName);

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

            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(request.IdentityId, customClaims, cancellationToken);
            await FirebaseAuth.DefaultInstance.RevokeRefreshTokensAsync(request.IdentityId, cancellationToken);

            _logger.LogInformation("Successfully removed role {RoleName} from user {IdentityId}", request.RoleName, request.IdentityId);

            return Result.Success("Role removed from user.");
        }
        catch (FirebaseAuthException ex)
        {
            if (ex.ErrorCode == ErrorCode.NotFound)
            {
                return Result.Failure(new Error("UserNotFound", "User not found in Firebase"));
            }

            _logger.LogError(ex, "Firebase error removing role {RoleName} from user {IdentityId}", request.RoleName, request.IdentityId);
            return Result.Failure(new Error("FirebaseError", "Failed to remove role from user"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error removing role {RoleName} from user {IdentityId}", request.RoleName, request.IdentityId);
            return Result.Failure(new Error("InternalError", "An unexpected error occurred"));
        }
    }
} 

public class RemoveUserRoleValidator : AbstractValidator<RemoveUserRoleCommand>
{
    public RemoveUserRoleValidator()
    {
        RuleFor(x => x.IdentityId).NotEmpty();
        RuleFor(x => x.RoleName).NotEmpty();
    }
}
