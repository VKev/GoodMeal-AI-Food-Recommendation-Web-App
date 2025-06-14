using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;
using FirebaseAdmin;

namespace Application.Auths.Queries.GetUserRolesQuery;

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

    public GetUserRolesQueryHandler(ILogger<GetUserRolesQueryHandler> logger)
    {
        _logger = logger;
    }

    public async Task<Result<GetUserRolesResponse>> Handle(GetUserRolesQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(request.IdentityId, cancellationToken);

            var roles = new List<string>();
            if (userRecord.CustomClaims != null && userRecord.CustomClaims.TryGetValue("roles", out var rolesObj))
            {
                if (rolesObj is string[] rolesArray)
                {
                    roles = rolesArray.ToList();
                }
            }

            var response = new GetUserRolesResponse(
                userRecord.Uid,
                userRecord.Email ?? "",
                userRecord.DisplayName ?? "",
                roles
            );

            return Result.Success(response);
        }
        catch (FirebaseAuthException ex)
        {
            if (ex.ErrorCode == ErrorCode.NotFound)
            {
                _logger.LogWarning("User {IdentityId} not found in Firebase", request.IdentityId);
                return Result.Failure<GetUserRolesResponse>(new Error("UserNotFound", "User not found in Firebase"));
            }

            _logger.LogError(ex, "Firebase error getting user roles for {IdentityId}", request.IdentityId);
            return Result.Failure<GetUserRolesResponse>(new Error("FirebaseError", "Failed to get user roles"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting user roles for {IdentityId}", request.IdentityId);
            return Result.Failure<GetUserRolesResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}