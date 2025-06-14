using FirebaseAdmin;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;
using FluentValidation;

namespace Application.Auths.Commands.EnableUserCommand;

public sealed record EnableUserCommand(string IdentityId) : ICommand;

internal sealed class EnableUserCommandHandler : ICommandHandler<EnableUserCommand>
{
    private readonly ILogger<EnableUserCommandHandler> _logger;

    public EnableUserCommandHandler(ILogger<EnableUserCommandHandler> logger)
    {
        _logger = logger;
    }

    public async Task<Result> Handle(EnableUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(request.IdentityId, cancellationToken);
            
            if (!userRecord.Disabled)
            {
                return Result.Failure(new Error("UserAlreadyEnabled", "User is already enabled"));
            }

            var args = new UserRecordArgs()
            {
                Uid = request.IdentityId,
                Disabled = false
            };

            await FirebaseAuth.DefaultInstance.UpdateUserAsync(args, cancellationToken);

            _logger.LogInformation("Successfully enabled user {IdentityId}", request.IdentityId);

            return Result.Success("User account enabled successfully.");
        }
        catch (FirebaseAuthException ex)
        {
            if (ex.ErrorCode == ErrorCode.NotFound)
            {
                return Result.Failure(new Error("UserNotFound", "User not found in Firebase"));
            }

            _logger.LogError(ex, "Firebase error enabling user {IdentityId}", request.IdentityId);
            return Result.Failure(new Error("FirebaseError", "Failed to enable user account"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error enabling user {IdentityId}", request.IdentityId);
            return Result.Failure(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}

public class EnableUserCommandValidator : AbstractValidator<EnableUserCommand>
{
    public EnableUserCommandValidator()
    {
        RuleFor(x => x.IdentityId).NotEmpty().WithMessage("Identity ID is required");
    }
} 