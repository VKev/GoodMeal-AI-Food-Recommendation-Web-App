using Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.Admin.Commands.DisableUserCommand;

public sealed record DisableUserCommand(string IdentityId) : ICommand;

internal sealed class DisableUserCommandHandler : ICommandHandler<DisableUserCommand>
{
    private readonly ILogger<DisableUserCommandHandler> _logger;
    private readonly IAuthenticationRepository _authenticationRepository;

    public DisableUserCommandHandler(ILogger<DisableUserCommandHandler> logger, IAuthenticationRepository authenticationRepository)
    {
        _logger = logger;
        _authenticationRepository = authenticationRepository;
    }

    public async Task<Result> Handle(DisableUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authenticationRepository.DisableUserAsync(request.IdentityId, cancellationToken);
            
            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to disable user {IdentityId}: {Error}", request.IdentityId, result.Error.Description);
                return Result.Failure(result.Error);
            }

            if (!result.Value.IsSuccess)
            {
                return Result.Failure(new Error(result.Value.ErrorCode ?? "DisableUserFailed", result.Value.Message));
            }

            _logger.LogInformation("Successfully disabled user {IdentityId}", request.IdentityId);
            return Result.Success(result.Value.Message);
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