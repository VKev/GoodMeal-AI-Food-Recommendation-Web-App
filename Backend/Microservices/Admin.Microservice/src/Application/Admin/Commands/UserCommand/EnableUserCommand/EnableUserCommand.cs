using Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.Admin.Commands.EnableUserCommand;

public sealed record EnableUserCommand(string IdentityId) : ICommand;

internal sealed class EnableUserCommandHandler : ICommandHandler<EnableUserCommand>
{
    private readonly ILogger<EnableUserCommandHandler> _logger;
    private readonly IAuthenticationRepository _authenticationRepository;

    public EnableUserCommandHandler(ILogger<EnableUserCommandHandler> logger, IAuthenticationRepository authenticationRepository)
    {
        _logger = logger;
        _authenticationRepository = authenticationRepository;
    }

    public async Task<Result> Handle(EnableUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authenticationRepository.EnableUserAsync(request.IdentityId, cancellationToken);
            
            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to enable user {IdentityId}: {Error}", request.IdentityId, result.Error.Description);
                return Result.Failure(result.Error);
            }

            if (!result.Value.IsSuccess)
            {
                return Result.Failure(new Error(result.Value.ErrorCode ?? "EnableUserFailed", result.Value.Message));
            }

            _logger.LogInformation("Successfully enabled user {IdentityId}", request.IdentityId);
            return Result.Success(result.Value.Message);
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