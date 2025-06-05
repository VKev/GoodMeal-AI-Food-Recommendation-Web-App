namespace src;

public class AuthenticationHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var context = _httpContextAccessor.HttpContext;
        var user = context.User;

        var isAuthenticated = user?.Identity?.IsAuthenticated == true;
        request.Headers.TryAddWithoutValidation("X-Is-Authenticated", isAuthenticated.ToString());

        if (user?.Identity?.IsAuthenticated == true)
        {
            var userId = user.GetFirebaseUserId();
            var email = user.GetEmail();
            var emailVerified = user.FindFirst("email_verified")?.Value;
            var signInProvider = user.GetSignInProvider();
            var roles = user.GetRoles();
            var primaryRole = user.GetPrimaryRole();

            if (!string.IsNullOrEmpty(userId))
                request.Headers.TryAddWithoutValidation("X-User-Id", userId);
            if (!string.IsNullOrEmpty(email))
                request.Headers.TryAddWithoutValidation("X-User-Email", email);
            if (!string.IsNullOrEmpty(emailVerified))
                request.Headers.TryAddWithoutValidation("X-Email-Verified", emailVerified);
            if (!string.IsNullOrEmpty(signInProvider))
                request.Headers.TryAddWithoutValidation("X-SignIn-Provider", signInProvider);
            if (!string.IsNullOrEmpty(primaryRole))
                request.Headers.TryAddWithoutValidation("X-User-Role", primaryRole);
            if (roles.Any())
                request.Headers.TryAddWithoutValidation("X-User-Roles", string.Join(",", roles));
        }

        return await base.SendAsync(request, cancellationToken);
    }
}