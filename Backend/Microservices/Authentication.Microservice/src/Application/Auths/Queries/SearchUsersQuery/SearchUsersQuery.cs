using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;
using FluentValidation;

namespace Application.Auths.Queries.SearchUsersQuery;

public sealed record SearchUsersQuery(string? SearchTerm, int PageSize = 50, string? PageToken = null)
    : IQuery<SearchUsersResponse>;

public sealed record UserInfo(
    string Uid,
    string Email,
    string DisplayName,
    bool IsDisabled,
    bool IsEmailVerified,
    DateTime? LastSignInTime,
    DateTime? CreationTime,
    List<string> Roles
);
public sealed record SearchUsersResponse(
    IEnumerable<UserInfo> Users,
    string? NextPageToken,
    int TotalCount,
    string SearchTerm
);

internal sealed class SearchUsersQueryHandler : IQueryHandler<SearchUsersQuery, SearchUsersResponse>
{
    private readonly ILogger<SearchUsersQueryHandler> _logger;

    public SearchUsersQueryHandler(ILogger<SearchUsersQueryHandler> logger)
    {
        _logger = logger;
    }

    public async Task<Result<SearchUsersResponse>> Handle(SearchUsersQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var users = new List<UserInfo>();
            var searchTerm = request.SearchTerm?.ToLower();
            string? nextPageToken = request.PageToken;

            var pagedEnumerable = FirebaseAuth.DefaultInstance.ListUsersAsync(new ListUsersOptions
            {
                PageSize = request.PageSize,
                PageToken = nextPageToken
            });

            var page = await pagedEnumerable.ReadPageAsync(request.PageSize, cancellationToken);

            await foreach (var userRecord in pagedEnumerable.WithCancellation(cancellationToken))
            {
                if (searchTerm != null)
                {
                    var matchesEmail = userRecord.Email?.ToLower().Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ?? false;
                    var matchesName = userRecord.DisplayName?.ToLower().Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ?? false;

                    if (!matchesEmail && !matchesName)
                        continue;
                }

                var roles = new List<string>();
                if (userRecord.CustomClaims != null && userRecord.CustomClaims.TryGetValue("roles", out var rolesObj))
                {
                    if (rolesObj is IEnumerable<object> rolesList)
                    {
                        roles = rolesList.Select(r => r.ToString() ?? "").ToList();
                    }
                }

                users.Add(new UserInfo(
                    userRecord.Uid,
                    userRecord.Email ?? "",
                    userRecord.DisplayName ?? "",
                    userRecord.Disabled,
                    userRecord.EmailVerified,
                    userRecord.UserMetaData?.LastSignInTimestamp,
                    userRecord.UserMetaData?.CreationTimestamp,
                    roles
                ));
            }

            var response = new SearchUsersResponse(
                users,
                page.NextPageToken,
                users.Count,
                request.SearchTerm ?? string.Empty
            );

            return Result.Success(response);
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogError(ex, "Firebase error searching users with term: {SearchTerm}", request.SearchTerm);
            return Result.Failure<SearchUsersResponse>(new Error("FirebaseError", "Failed to search users"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error searching users with term: {SearchTerm}", request.SearchTerm);
            return Result.Failure<SearchUsersResponse>(new Error("InternalError", "An unexpected error occurred"));
        }
    }
}

public class SearchUsersQueryValidator : AbstractValidator<SearchUsersQuery>
{
    public SearchUsersQueryValidator()
    {
        RuleFor(x => x.SearchTerm).MinimumLength(2).When(x => !string.IsNullOrEmpty(x.SearchTerm)).WithMessage("Search term must be at least 2 characters");
        RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(1000)
            .WithMessage("Page size must be between 1 and 1000");
    }
}