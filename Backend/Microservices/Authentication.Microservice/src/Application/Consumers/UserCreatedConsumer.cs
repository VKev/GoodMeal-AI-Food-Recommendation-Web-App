using FirebaseAdmin;
using MassTransit;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;
using SharedLibrary.Contracts.UserCreating;

namespace Application.Consumers;

public class UserCreatedConsumer : IConsumer<UserCreatedEvent>
{
    private readonly ILogger<UserCreatedConsumer> _logger;

    public UserCreatedConsumer(ILogger<UserCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        var message = context.Message;
        
        try
        {
            _logger.LogInformation("Processing user created event for user {IdentityId} with system user ID {UserId}", 
                message.IdentityId, message.UserId);

            var customClaims = new Dictionary<string, object>
            {
                ["email"] = message.Email,
                ["name"] = message.Name,
                ["user_id"] = message.IdentityId,
                ["system_user_id"] = message.UserId.ToString(),
                ["roles"] = new[] { "User" }, // Default role for new users
                ["created_at"] = DateTime.UtcNow.ToString("O"),
                ["created_by"] = "System"
            };

            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(message.IdentityId, customClaims);
            
            try
            {
                await FirebaseAuth.DefaultInstance.RevokeRefreshTokensAsync(message.IdentityId);
                _logger.LogInformation("Successfully set initial custom claims for user {IdentityId} with system_user_id {UserId}", 
                    message.IdentityId, message.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to revoke refresh tokens for user {IdentityId}, but custom claims were updated", 
                    message.IdentityId);
            }
        }
        catch (FirebaseAuthException ex)
        {
            if (ex.ErrorCode == ErrorCode.NotFound)
            {
                _logger.LogWarning("User {IdentityId} not found in Firebase, skipping claims update", message.IdentityId);
                return;
            }
            
            _logger.LogError(ex, "Firebase error updating custom claims for user {IdentityId}", message.IdentityId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating custom claims for user {IdentityId}", message.IdentityId);
            throw; 
        }
    }
} 