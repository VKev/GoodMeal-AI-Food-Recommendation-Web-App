using Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.Admin.Queries.GetUserStatusQuery;

public sealed record GetUserStatusQuery(string IdentityId) : IQuery<GetUserStatusResponse>;

public sealed record GetUserStatusResponse(
    string IdentityId,
    string Email,
    string Name,
    bool IsDisabled,
    bool EmailVerified,
    DateTime? LastSignInTime,
    DateTime? CreationTime
);

internal sealed class GetUserStatusQueryHandler : IQueryHandler<GetUserStatusQuery, GetUserStatusResponse>
{
    private readonly ILogger<GetUserStatusQueryHandler> _logger;
    private readonly IAuthenticationRepository _authenticationRepository;

    public GetUserStatusQueryHandler(ILogger<GetUserStatusQueryHandler> logger, IAuthenticationRepository authenticationRepository)
    {
        _logger = logger;
        _authenticationRepository = authenticationRepository;
    }

    public async Task<Result<GetUserStatusResponse>> Handle(GetUserStatusQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authenticationRepository.GetUserStatusAsync(request.IdentityId, cancellationToken);
            
            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to get user status for {IdentityId}: {Error}", request.IdentityId, result.Error.Description);
                return Result.Failure<GetUserStatusResponse>(result.Error);
            }

            var response = new GetUserStatusResponse(
                result.Value.IdentityId,
                result.Value.Email,
                result.Value.Name,
                result.Value.IsDisabled,
                result.Value.EmailVerified,
                result.Value.LastSignInTime,
                result.Value.CreationTime
            );

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting user status for {IdentityId}", request.IdentityId);
            return Result.Failure<GetUserStatusResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}

public class GetUserStatusQueryValidator : AbstractValidator<GetUserStatusQuery>
{
    public GetUserStatusQueryValidator()
    {
        RuleFor(x => x.IdentityId).NotEmpty().WithMessage("Identity ID is required");
    }
} 