using FirebaseAdmin;
using FirebaseAdmin.Auth;
using MassTransit;
using Microsoft.Extensions.Logging;
using SharedLibrary.Contracts.Authentication;

namespace Application.Consumers;

public class EnableUserConsumer : IConsumer<EnableUserRequest>
{
    private readonly ILogger<EnableUserConsumer> _logger;

    public EnableUserConsumer(ILogger<EnableUserConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<EnableUserRequest> context)
    {
        try
        {
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(context.Message.IdentityId, context.CancellationToken);
            
            if (!userRecord.Disabled)
            {
                await context.RespondAsync(new UserCommandResponse 
                { 
                    IsSuccess = false, 
                    Message = "User is already enabled",
                    ErrorCode = "UserAlreadyEnabled"
                });
                return;
            }

            var args = new UserRecordArgs()
            {
                Uid = context.Message.IdentityId,
                Disabled = false
            };

            await FirebaseAuth.DefaultInstance.UpdateUserAsync(args, context.CancellationToken);

            _logger.LogInformation("Successfully enabled user {IdentityId}", context.Message.IdentityId);

            await context.RespondAsync(new UserCommandResponse 
            { 
                IsSuccess = true, 
                Message = "User account enabled successfully."
            });
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogError(ex, "Firebase error enabling user {IdentityId}", context.Message.IdentityId);
            
            await context.RespondAsync(new UserCommandResponse 
            { 
                IsSuccess = false, 
                Message = ex.ErrorCode == ErrorCode.NotFound ? "User not found in Firebase" : "Failed to enable user account",
                ErrorCode = ex.ErrorCode.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error enabling user {IdentityId}", context.Message.IdentityId);
            throw;
        }
    }
}

public class DisableUserConsumer : IConsumer<DisableUserRequest>
{
    private readonly ILogger<DisableUserConsumer> _logger;

    public DisableUserConsumer(ILogger<DisableUserConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DisableUserRequest> context)
    {
        try
        {
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(context.Message.IdentityId, context.CancellationToken);
            
            if (userRecord.Disabled)
            {
                await context.RespondAsync(new UserCommandResponse 
                { 
                    IsSuccess = false, 
                    Message = "User is already disabled",
                    ErrorCode = "UserAlreadyDisabled"
                });
                return;
            }

            var args = new UserRecordArgs()
            {
                Uid = context.Message.IdentityId,
                Disabled = true
            };

            await FirebaseAuth.DefaultInstance.UpdateUserAsync(args, context.CancellationToken);

            _logger.LogInformation("Successfully disabled user {IdentityId}", context.Message.IdentityId);

            await context.RespondAsync(new UserCommandResponse 
            { 
                IsSuccess = true, 
                Message = "User account disabled successfully."
            });
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogError(ex, "Firebase error disabling user {IdentityId}", context.Message.IdentityId);
            
            await context.RespondAsync(new UserCommandResponse 
            { 
                IsSuccess = false, 
                Message = ex.ErrorCode == ErrorCode.NotFound ? "User not found in Firebase" : "Failed to disable user account",
                ErrorCode = ex.ErrorCode.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error disabling user {IdentityId}", context.Message.IdentityId);
            throw;
        }
    }
}

public class DeleteUserConsumer : IConsumer<DeleteUserRequest>
{
    private readonly ILogger<DeleteUserConsumer> _logger;

    public DeleteUserConsumer(ILogger<DeleteUserConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DeleteUserRequest> context)
    {
        try
        {
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(context.Message.IdentityId, context.CancellationToken);

            await FirebaseAuth.DefaultInstance.DeleteUserAsync(context.Message.IdentityId, context.CancellationToken);

            _logger.LogInformation("Successfully deleted user {IdentityId} with email {Email}", 
                context.Message.IdentityId, userRecord.Email);

            await context.RespondAsync(new UserCommandResponse 
            { 
                IsSuccess = true, 
                Message = "User deleted successfully."
            });
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogError(ex, "Firebase error deleting user {IdentityId}", context.Message.IdentityId);
            
            await context.RespondAsync(new UserCommandResponse 
            { 
                IsSuccess = false, 
                Message = ex.ErrorCode == ErrorCode.NotFound ? "User not found in Firebase" : "Failed to delete user",
                ErrorCode = ex.ErrorCode.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting user {IdentityId}", context.Message.IdentityId);
            throw;
        }
    }
}

public class UpdateUserConsumer : IConsumer<UpdateUserRequest>
{
    private readonly ILogger<UpdateUserConsumer> _logger;

    public UpdateUserConsumer(ILogger<UpdateUserConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UpdateUserRequest> context)
    {
        try
        {
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(context.Message.IdentityId, context.CancellationToken);

            var args = new UserRecordArgs()
            {
                Uid = context.Message.IdentityId
            };

            bool hasChanges = false;

            if (!string.IsNullOrEmpty(context.Message.Email) && context.Message.Email != userRecord.Email)
            {
                args.Email = context.Message.Email;
                hasChanges = true;
            }

            if (!string.IsNullOrEmpty(context.Message.DisplayName) && context.Message.DisplayName != userRecord.DisplayName)
            {
                args.DisplayName = context.Message.DisplayName;
                hasChanges = true;
            }

            if (context.Message.EmailVerified.HasValue && context.Message.EmailVerified.Value != userRecord.EmailVerified)
            {
                args.EmailVerified = context.Message.EmailVerified.Value;
                hasChanges = true;
            }

            if (!hasChanges)
            {
                await context.RespondAsync(new UpdateUserResponse 
                { 
                    IsSuccess = false, 
                    Message = "No changes were made to the user"
                });
                return;
            }

            await FirebaseAuth.DefaultInstance.UpdateUserAsync(args, context.CancellationToken);

            _logger.LogInformation("Successfully updated user {IdentityId}", context.Message.IdentityId);

            await context.RespondAsync(new UpdateUserResponse 
            { 
                IsSuccess = true, 
                Message = "User updated successfully."
            });
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogError(ex, "Firebase error updating user {IdentityId}", context.Message.IdentityId);
            
            var errorMessage = ex.ErrorCode == ErrorCode.NotFound ? "User not found in Firebase" 
                : ex.ErrorCode == ErrorCode.InvalidArgument && ex.Message.Contains("email") ? "The email address is invalid or already in use"
                : "Failed to update user";
                
            await context.RespondAsync(new UpdateUserResponse 
            { 
                IsSuccess = false, 
                Message = errorMessage
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating user {IdentityId}", context.Message.IdentityId);
            throw;
        }
    }
} 