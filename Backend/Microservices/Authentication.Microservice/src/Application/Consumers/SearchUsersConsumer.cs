using FirebaseAdmin;
using FirebaseAdmin.Auth;
using MassTransit;
using Microsoft.Extensions.Logging;
using SharedLibrary.Contracts.Authentication;

namespace Application.Consumers;

public class SearchUsersConsumer : IConsumer<SearchUsersRequest>
{
    private readonly ILogger<SearchUsersConsumer> _logger;

    public SearchUsersConsumer(ILogger<SearchUsersConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SearchUsersRequest> context)
    {
        try
        {
            var users = new List<UserInfo>();
            var searchTerm = context.Message.SearchTerm?.ToLower();
            string? nextPageToken = context.Message.PageToken;

            var pagedEnumerable = FirebaseAuth.DefaultInstance.ListUsersAsync(new ListUsersOptions
            {
                PageSize = context.Message.PageSize,
                PageToken = nextPageToken
            });

            var page = await pagedEnumerable.ReadPageAsync(context.Message.PageSize, context.CancellationToken);

            await foreach (var userRecord in pagedEnumerable.WithCancellation(context.CancellationToken))
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

                users.Add(new UserInfo
                {
                    Uid = userRecord.Uid,
                    Email = userRecord.Email ?? "",
                    DisplayName = userRecord.DisplayName ?? "",
                    IsDisabled = userRecord.Disabled,
                    IsEmailVerified = userRecord.EmailVerified,
                    LastSignInTime = userRecord.UserMetaData?.LastSignInTimestamp,
                    CreationTime = userRecord.UserMetaData?.CreationTimestamp,
                    Roles = roles
                });
            }

            var response = new SearchUsersResponse
            {
                Users = users,
                NextPageToken = page.NextPageToken,
                TotalCount = users.Count,
                SearchTerm = context.Message.SearchTerm ?? string.Empty
            };

            await context.RespondAsync(response);
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogError(ex, "Firebase error searching users with term: {SearchTerm}", context.Message.SearchTerm);
            
            await context.RespondAsync(new SearchUsersResponse 
            { 
                Users = new List<UserInfo>(),
                NextPageToken = null,
                TotalCount = 0,
                SearchTerm = context.Message.SearchTerm ?? string.Empty
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error searching users with term: {SearchTerm}", context.Message.SearchTerm);
            throw;
        }
    }
} 