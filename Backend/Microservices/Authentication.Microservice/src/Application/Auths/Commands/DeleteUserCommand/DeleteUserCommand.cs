using FirebaseAdmin;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;
using FluentValidation;

namespace Application.Auths.Commands.DeleteUserCommand;

public sealed record DeleteUserCommand(string IdentityId) : ICommand;

internal sealed class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
    private readonly ILogger<DeleteUserCommandHandler> _logger;

    public DeleteUserCommandHandler(ILogger<DeleteUserCommandHandler> logger)
    {
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // First check if user exists
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(request.IdentityId, cancellationToken);

            await FirebaseAuth.DefaultInstance.DeleteUserAsync(request.IdentityId, cancellationToken);

            _logger.LogInformation("Successfully deleted user {IdentityId} with email {Email}", 
                request.IdentityId, userRecord.Email);

            return Result.Success("User deleted successfully.");
        }
        catch (FirebaseAuthException ex)
        {
            if (ex.ErrorCode == ErrorCode.NotFound)
            {
                return Result.Failure(new Error("UserNotFound", "User not found in Firebase"));
            }

            _logger.LogError(ex, "Firebase error deleting user {IdentityId}", request.IdentityId);
            return Result.Failure(new Error("FirebaseError", "Failed to delete user"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting user {IdentityId}", request.IdentityId);
            return Result.Failure(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.IdentityId).NotEmpty().WithMessage("Identity ID is required");
    }
} 