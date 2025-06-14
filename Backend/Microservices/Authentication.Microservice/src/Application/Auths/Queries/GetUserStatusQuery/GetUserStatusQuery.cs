using FirebaseAdmin;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;
using FluentValidation;

namespace Application.Auths.Queries.GetUserStatusQuery;

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

    public GetUserStatusQueryHandler(ILogger<GetUserStatusQueryHandler> logger)
    {
        _logger = logger;
    }

    public async Task<Result<GetUserStatusResponse>> Handle(GetUserStatusQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(request.IdentityId, cancellationToken);

            var response = new GetUserStatusResponse(
                userRecord.Uid,
                userRecord.Email ?? "",
                userRecord.DisplayName ?? "",
                userRecord.Disabled,
                userRecord.EmailVerified,
                userRecord.UserMetaData?.LastSignInTimestamp,
                userRecord.UserMetaData?.CreationTimestamp
            );

            return Result.Success(response);
        }
        catch (FirebaseAuthException ex)
        {
            if (ex.ErrorCode == ErrorCode.NotFound)
            {
                _logger.LogWarning("User {IdentityId} not found in Firebase", request.IdentityId);
                return Result.Failure<GetUserStatusResponse>(new Error("UserNotFound", "User not found in Firebase"));
            }

            _logger.LogError(ex, "Firebase error getting user status for {IdentityId}", request.IdentityId);
            return Result.Failure<GetUserStatusResponse>(new Error("FirebaseError", "Failed to get user status"));
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