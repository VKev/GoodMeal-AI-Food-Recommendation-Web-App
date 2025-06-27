using Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.Admin.Commands.DeleteUserCommand;

public sealed record DeleteUserCommand(string IdentityId) : ICommand;

internal sealed class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
    private readonly ILogger<DeleteUserCommandHandler> _logger;
    private readonly IAuthenticationRepository _authenticationRepository;

    public DeleteUserCommandHandler(ILogger<DeleteUserCommandHandler> logger, IAuthenticationRepository authenticationRepository)
    {
        _logger = logger;
        _authenticationRepository = authenticationRepository;
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authenticationRepository.DeleteUserAsync(request.IdentityId, cancellationToken);
            
            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to delete user {IdentityId}: {Error}", request.IdentityId, result.Error.Description);
                return Result.Failure(result.Error);
            }

            if (!result.Value.IsSuccess)
            {
                return Result.Failure(new Error(result.Value.ErrorCode ?? "DeleteUserFailed", result.Value.Message));
            }

            _logger.LogInformation("Successfully deleted user {IdentityId}", request.IdentityId);
            return Result.Success(result.Value.Message);
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