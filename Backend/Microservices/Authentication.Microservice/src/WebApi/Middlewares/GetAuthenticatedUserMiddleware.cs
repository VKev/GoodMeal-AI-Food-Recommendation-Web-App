using System.Security.Claims;

namespace WebApi.Middlewares;

public class GetAuthenticatedUserMiddleware
{
    private readonly RequestDelegate _next;

    public GetAuthenticatedUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var headers = context.Request.Headers;

        var userId = headers["X-User-Id"].FirstOrDefault();
        var email = headers["X-User-Email"].FirstOrDefault();
        var role = headers["X-User-Role"].FirstOrDefault();

        if (!string.IsNullOrEmpty(userId))
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email ?? ""),
                new Claim(ClaimTypes.Role, role ?? "")
            };

            var identity = new ClaimsIdentity(claims, "Gateway");
            context.User = new ClaimsPrincipal(identity);
        }

        await _next(context);
    }
}