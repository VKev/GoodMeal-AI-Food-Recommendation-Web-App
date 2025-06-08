using FirebaseAdmin;
using MassTransit;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;
using SharedLibrary.Contracts.UserRoleChanged;
using Domain.Services;

namespace Application.Consumers;

public class ControlAccessCustomRoleClaimConsumer : IConsumer<UserRoleChangedEvent>
{
    private readonly ILogger<ControlAccessCustomRoleClaimConsumer> _logger;
    private readonly IUserService _userService;

    public ControlAccessCustomRoleClaimConsumer(ILogger<ControlAccessCustomRoleClaimConsumer> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    public async Task Consume(ConsumeContext<UserRoleChangedEvent> context)
    {
        var message = context.Message;

        try
        {
            var customClaims = new Dictionary<string, object>
            {
                ["email"] = message.Email,
                ["name"] = message.Name,
                ["roles"] = message.NewRoles.ToArray(),
                ["user_id"] = message.IdentityId,
                ["updated_at"] = message.ChangedAt.ToString("O"),
                ["updated_by"] = message.ChangedBy
            };

            var userRoles = await _userService.GetUserRolesAsync(message.IdentityId, context.CancellationToken);
            if (userRoles?.UserId != null && userRoles.UserId != Guid.Empty)
            {
                customClaims["system_user_id"] = userRoles.UserId.ToString();
            }

            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(message.IdentityId, customClaims);

            try
            {
                await FirebaseAuth.DefaultInstance.RevokeRefreshTokensAsync(message.IdentityId);
                _logger.LogInformation("Revoked refresh tokens for user {IdentityId} to force token refresh", message.IdentityId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to revoke refresh tokens for user {IdentityId}, but custom claims were updated", message.IdentityId);
            }
        }
        catch (FirebaseAuthException ex)
        {
            if (ex.ErrorCode == ErrorCode.NotFound)
            {
                _logger.LogWarning("User {IdentityId} not found in Firebase, skipping role update", message.IdentityId);
                return;
            }

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating custom claims for user {IdentityId}", message.IdentityId);
            throw;
        }
    }
}