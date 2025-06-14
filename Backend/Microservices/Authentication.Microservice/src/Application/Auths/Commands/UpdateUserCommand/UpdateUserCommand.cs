using FirebaseAdmin;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;
using FluentValidation;

namespace Application.Auths.Commands.UpdateUserCommand;

public sealed record UpdateUserCommand(
    string IdentityId,
    string? Email = null,
    string? DisplayName = null,
    bool? EmailVerified = null
) : ICommand;

internal sealed class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
{
    private readonly ILogger<UpdateUserCommandHandler> _logger;

    public UpdateUserCommandHandler(ILogger<UpdateUserCommandHandler> logger)
    {
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(request.IdentityId, cancellationToken);

            var args = new UserRecordArgs()
            {
                Uid = request.IdentityId
            };

            bool hasChanges = false;

            if (!string.IsNullOrEmpty(request.Email) && request.Email != userRecord.Email)
            {
                args.Email = request.Email;
                hasChanges = true;
            }

            if (!string.IsNullOrEmpty(request.DisplayName) && request.DisplayName != userRecord.DisplayName)
            {
                args.DisplayName = request.DisplayName;
                hasChanges = true;
            }

            if (request.EmailVerified.HasValue && request.EmailVerified.Value != userRecord.EmailVerified)
            {
                args.EmailVerified = request.EmailVerified.Value;
                hasChanges = true;
            }

            if (!hasChanges)
            {
                return Result.Failure(new Error("NoChanges", "No changes were made to the user"));
            }

            await FirebaseAuth.DefaultInstance.UpdateUserAsync(args, cancellationToken);

            _logger.LogInformation("Successfully updated user {IdentityId}", request.IdentityId);

            return Result.Success("User updated successfully.");
        }
        catch (FirebaseAuthException ex)
        {
            if (ex.ErrorCode == ErrorCode.NotFound)
            {
                return Result.Failure(new Error("UserNotFound", "User not found in Firebase"));
            }
            if (ex.ErrorCode == ErrorCode.InvalidArgument && ex.Message.Contains("email"))
            {
                return Result.Failure(new Error("InvalidEmail", "The email address is invalid or already in use"));
            }

            _logger.LogError(ex, "Firebase error updating user {IdentityId}", request.IdentityId);
            return Result.Failure(new Error("FirebaseError", "Failed to update user"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating user {IdentityId}", request.IdentityId);
            return Result.Failure(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.IdentityId).NotEmpty().WithMessage("Identity ID is required");
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Valid email address is required");
        RuleFor(x => x.DisplayName).MinimumLength(2).MaximumLength(100).When(x => !string.IsNullOrEmpty(x.DisplayName))
            .WithMessage("Display name must be between 2 and 100 characters");
    }
}