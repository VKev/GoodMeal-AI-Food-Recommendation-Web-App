using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Application.Services;

namespace Application.Admin.Commands.RemoveUserRoleCommand;

public sealed record RemoveUserRoleCommand(string IdentityId, string RoleName) : ICommand;

internal sealed class RemoveUserRoleCommandHandler : ICommandHandler<RemoveUserRoleCommand>
{
    private readonly ILogger<RemoveUserRoleCommandHandler> _logger;
    private readonly IAuthenticationRepository _authenticationRepository;

    public RemoveUserRoleCommandHandler(ILogger<RemoveUserRoleCommandHandler> logger, IAuthenticationRepository authenticationRepository)
    {
        _logger = logger;
        _authenticationRepository = authenticationRepository;
    }

    public async Task<Result> Handle(RemoveUserRoleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authenticationRepository.RemoveUserRoleAsync(request.IdentityId, request.RoleName, cancellationToken);
            
            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to remove role {RoleName} from user {IdentityId}: {Error}", request.RoleName, request.IdentityId, result.Error.Description);
                return Result.Failure(result.Error);
            }

            if (!result.Value.IsSuccess)
            {
                return Result.Failure(new Error(result.Value.ErrorCode ?? "RemoveRoleFailed", result.Value.Message));
            }

            _logger.LogInformation("Successfully removed role {RoleName} from user {IdentityId}", request.RoleName, request.IdentityId);
            return Result.Success(result.Value.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error removing role {RoleName} from user {IdentityId}", request.RoleName, request.IdentityId);
            return Result.Failure(new Error("InternalError", "An unexpected error occurred"));
        }
    }
} 

public class RemoveUserRoleValidator : AbstractValidator<RemoveUserRoleCommand>
{
    public RemoveUserRoleValidator()
    {
        RuleFor(x => x.IdentityId).NotEmpty();
        RuleFor(x => x.RoleName).NotEmpty();
    }
}
