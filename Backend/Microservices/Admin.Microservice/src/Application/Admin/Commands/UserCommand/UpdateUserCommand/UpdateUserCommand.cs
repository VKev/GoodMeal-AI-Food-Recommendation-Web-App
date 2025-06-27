using Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.Admin.Commands.UpdateUserCommand;

public sealed record UpdateUserCommand(
    string IdentityId,
    string? Email = null,
    string? DisplayName = null,
    bool? EmailVerified = null
) : ICommand;

internal sealed class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
{
    private readonly ILogger<UpdateUserCommandHandler> _logger;
    private readonly IAuthenticationRepository _authenticationRepository;

    public UpdateUserCommandHandler(ILogger<UpdateUserCommandHandler> logger, IAuthenticationRepository authenticationRepository)
    {
        _logger = logger;
        _authenticationRepository = authenticationRepository;
    }

    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authenticationRepository.UpdateUserAsync(
                request.IdentityId, 
                request.Email, 
                request.DisplayName, 
                request.EmailVerified, 
                cancellationToken);
            
            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to update user {IdentityId}: {Error}", request.IdentityId, result.Error.Description);
                return Result.Failure(result.Error);
            }

            if (!result.Value.IsSuccess)
            {
                return Result.Failure(new Error("UpdateUserFailed", result.Value.Message));
            }

            _logger.LogInformation("Successfully updated user {IdentityId}", request.IdentityId);
            return Result.Success(result.Value.Message);
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