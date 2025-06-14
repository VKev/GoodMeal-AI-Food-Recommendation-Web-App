using SharedLibrary.Common.Messaging;
using FluentValidation;
using SharedLibrary.Common.ResponseModel;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;
using FirebaseAdmin;

namespace Application.Auths.Commands.AddUserRoleCommand;

public sealed record AddUserRoleCommand(string IdentityId, string RoleName) : ICommand;

internal sealed class AddUserRoleCommandHandler : ICommandHandler<AddUserRoleCommand>
{
    private readonly ILogger<AddUserRoleCommandHandler> _logger;

    public AddUserRoleCommandHandler(ILogger<AddUserRoleCommandHandler> logger)
    {
        _logger = logger;
    }

    public async Task<Result> Handle(AddUserRoleCommand request, CancellationToken cancellationToken)
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

            if (currentRoles.Contains(request.RoleName))
            {
                return Result.Failure(new Error("RoleAlreadyExists", "User already has this role"));
            }

            currentRoles.Add(request.RoleName);

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

            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(request.IdentityId, customClaims,
                cancellationToken);

            await FirebaseAuth.DefaultInstance.RevokeRefreshTokensAsync(request.IdentityId, cancellationToken);

            _logger.LogInformation("Successfully added role {RoleName} to user {IdentityId}", request.RoleName,
                request.IdentityId);

            return Result.Success();
        }
        catch (FirebaseAuthException ex)
        {
            if (ex.ErrorCode == ErrorCode.NotFound)
            {
                return Result.Failure(new Error("UserNotFound", "User not found in Firebase"));
            }

            _logger.LogError(ex, "Firebase error adding role {RoleName} to user {IdentityId}", request.RoleName,
                request.IdentityId);
            return Result.Failure(new Error("FirebaseError", "Failed to add role to user"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error adding role {RoleName} to user {IdentityId}", request.RoleName,
                request.IdentityId);
            return Result.Failure(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}

public class AddUserRoleValidator : AbstractValidator<AddUserRoleCommand>
{
    public AddUserRoleValidator()
    {
        RuleFor(x => x.IdentityId).NotEmpty();
        RuleFor(x => x.RoleName).NotEmpty();
    }
}