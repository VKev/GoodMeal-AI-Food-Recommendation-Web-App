using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using Application.Services;

namespace Application.Admin.Commands.AddUserRoleCommand;

public sealed record AddUserRoleCommand(string IdentityId, string RoleName) : ICommand;

internal sealed class AddUserRoleCommandHandler : ICommandHandler<AddUserRoleCommand>
{
    private readonly ILogger<AddUserRoleCommandHandler> _logger;
    private readonly IAuthenticationRepository _authenticationRepository;

    public AddUserRoleCommandHandler(ILogger<AddUserRoleCommandHandler> logger, IAuthenticationRepository authenticationRepository)
    {
        _logger = logger;
        _authenticationRepository = authenticationRepository;
    }

    public async Task<Result> Handle(AddUserRoleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authenticationRepository.AddUserRoleAsync(request.IdentityId, request.RoleName, cancellationToken);
            
            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to add role {RoleName} to user {IdentityId}: {Error}", request.RoleName, request.IdentityId, result.Error.Description);
                return Result.Failure(result.Error);
            }

            if (!result.Value.IsSuccess)
            {
                return Result.Failure(new Error(result.Value.ErrorCode ?? "AddRoleFailed", result.Value.Message));
            }

            _logger.LogInformation("Successfully added role {RoleName} to user {IdentityId}", request.RoleName, request.IdentityId);
            return Result.Success(result.Value.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error adding role {RoleName} to user {IdentityId}", request.RoleName, request.IdentityId);
            return Result.Failure(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}

public class AddUserRoleValidator : AbstractValidator<AddUserRoleCommand>
{
    public AddUserRoleValidator()
    {
        RuleFor(x => x.IdentityId).NotEmpty();
        RuleFor(x => x.RoleName).NotEmpty();
    }
}