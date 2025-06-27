using Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.Admin.Queries.SearchUsersQuery;

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
    private readonly IAuthenticationRepository _authenticationRepository;

    public SearchUsersQueryHandler(ILogger<SearchUsersQueryHandler> logger, IAuthenticationRepository authenticationRepository)
    {
        _logger = logger;
        _authenticationRepository = authenticationRepository;
    }

    public async Task<Result<SearchUsersResponse>> Handle(SearchUsersQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authenticationRepository.SearchUsersAsync(
                request.SearchTerm, 
                request.PageSize, 
                request.PageToken, 
                cancellationToken);
            
            if (result.IsFailure)
            {
                _logger.LogWarning("Failed to search users with term {SearchTerm}: {Error}", request.SearchTerm, result.Error.Description);
                return Result.Failure<SearchUsersResponse>(result.Error);
            }

            var users = result.Value.Users.Select(u => new UserInfo(
                u.Uid,
                u.Email,
                u.DisplayName,
                u.IsDisabled,
                u.IsEmailVerified,
                u.LastSignInTime,
                u.CreationTime,
                u.Roles.ToList()
            ));

            var response = new SearchUsersResponse(
                users,
                result.Value.NextPageToken,
                result.Value.TotalCount,
                result.Value.SearchTerm
            );

            return Result.Success(response);
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