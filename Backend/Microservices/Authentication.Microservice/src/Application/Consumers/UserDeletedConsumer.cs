using FirebaseAdmin;
using MassTransit;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;
using SharedLibrary.Contracts.UserDeleted;

namespace Application.Consumers;

public class UserDeletedConsumer : IConsumer<UserDeletedEvent>
{
    private readonly ILogger<UserDeletedConsumer> _logger;

    public UserDeletedConsumer(ILogger<UserDeletedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserDeletedEvent> context)
    {
        var message = context.Message;

        try
        {
            _logger.LogInformation("Processing user deleted event for user {IdentityId} with system user ID {UserId}",
                message.IdentityId, message.UserId);

            var updateRequest = new UserRecordArgs
            {
                Uid = message.IdentityId,
                Disabled = true
            };

            await FirebaseAuth.DefaultInstance.UpdateUserAsync(updateRequest);

            var customClaims = new Dictionary<string, object>
            {
                ["email"] = message.Email,
                ["name"] = message.Name,
                ["user_id"] = message.IdentityId,
                ["system_user_id"] = message.UserId.ToString(),
                ["is_deleted"] = true,
                ["deleted_at"] = message.DeletedAt.ToString("O"),
                ["deleted_by"] = message.DeletedBy,
                ["roles"] = new string[] { }
            };

            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(message.IdentityId, customClaims);

            try
            {
                await FirebaseAuth.DefaultInstance.RevokeRefreshTokensAsync(message.IdentityId);
                _logger.LogInformation("Successfully disabled Firebase account and revoked tokens for user {IdentityId} with system_user_id {UserId}",
                    message.IdentityId, message.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to revoke refresh tokens for user {IdentityId}, but account was disabled",
                    message.IdentityId);
            }
        }
        catch (FirebaseAuthException ex)
        {
            if (ex.ErrorCode == ErrorCode.NotFound)
            {
                _logger.LogWarning("User {IdentityId} not found in Firebase, skipping account disable", message.IdentityId);
                return;
            }

            _logger.LogError(ex, "Firebase error disabling account for user {IdentityId}", message.IdentityId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error disabling account for user {IdentityId}", message.IdentityId);
            throw;
        }
    }
}