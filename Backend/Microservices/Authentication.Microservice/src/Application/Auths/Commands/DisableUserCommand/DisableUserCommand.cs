using FirebaseAdmin;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;
using FluentValidation;

namespace Application.Auths.Commands.DisableUserCommand;

public sealed record DisableUserCommand(string IdentityId) : ICommand;

internal sealed class DisableUserCommandHandler : ICommandHandler<DisableUserCommand>
{
    private readonly ILogger<DisableUserCommandHandler> _logger;

    public DisableUserCommandHandler(ILogger<DisableUserCommandHandler> logger)
    {
        _logger = logger;
    }

    public async Task<Result> Handle(DisableUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(request.IdentityId, cancellationToken);
            
            if (userRecord.Disabled)
            {
                return Result.Failure(new Error("UserAlreadyDisabled", "User is already disabled"));
            }

            var args = new UserRecordArgs()
            {
                Uid = request.IdentityId,
                Disabled = true
            };

            await FirebaseAuth.DefaultInstance.UpdateUserAsync(args, cancellationToken);

            _logger.LogInformation("Successfully disabled user {IdentityId}", request.IdentityId);

            return Result.Success("User account disabled successfully.");
        }
        catch (FirebaseAuthException ex)
        {
            if (ex.ErrorCode == ErrorCode.NotFound)
            {
                return Result.Failure(new Error("UserNotFound", "User not found in Firebase"));
            }

            _logger.LogError(ex, "Firebase error disabling user {IdentityId}", request.IdentityId);
            return Result.Failure(new Error("FirebaseError", "Failed to disable user account"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error disabling user {IdentityId}", request.IdentityId);
            return Result.Failure(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}

public class DisableUserCommandValidator : AbstractValidator<DisableUserCommand>
{
    public DisableUserCommandValidator()
    {
        RuleFor(x => x.IdentityId).NotEmpty().WithMessage("Identity ID is required");
    }
} 