using Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.Admin.Queries.GetUserRolesQuery;

public sealed record GetUserRolesQuery(string IdentityId) : IQuery<GetUserRolesResponse>;

public sealed record GetUserRolesResponse(
    string IdentityId,
    string Email,
    string Name,
    IEnumerable<string> Roles
);

internal sealed class GetUserRolesQueryHandler : IQueryHandler<GetUserRolesQuery, GetUserRolesResponse>
{
    private readonly ILogger<GetUserRolesQueryHandler> _logger;
    private readonly IAuthenticationRepository _authenticationRepository;

    public GetUserRolesQueryHandler(ILogger<GetUserRolesQueryHandler> logger, IAuthenticationRepository authenticationRepository)
    {
        _logger = logger;
        _authenticationRepository = authenticationRepository;
    }

    public async Task<Result<GetUserRolesResponse>> Handle(GetUserRolesQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authenticationRepository.GetUserRolesAsync(request.IdentityId, cancellationToken);
            
            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to get user roles for {IdentityId}: {Error}", request.IdentityId, result.Error.Description);
                return Result.Failure<GetUserRolesResponse>(result.Error);
            }

            var response = new GetUserRolesResponse(
                result.Value.IdentityId,
                result.Value.Email,
                result.Value.Name,
                result.Value.Roles
            );

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting user roles for {IdentityId}", request.IdentityId);
            return Result.Failure<GetUserRolesResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}