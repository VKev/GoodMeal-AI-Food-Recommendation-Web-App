using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SharedLibrary.Utils;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiGatewayUserAttribute : Attribute, IActionFilter
{
    public string? Roles { get; set; }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var headers = context.HttpContext.Request.Headers;

        var authenticated = headers["X-Is-Authenticated"].FirstOrDefault();
        var userId = headers["X-User-Id"].FirstOrDefault();

        if (string.IsNullOrEmpty(authenticated) || string.IsNullOrEmpty(userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (!bool.TryParse(authenticated, out var isAuthenticated) || !isAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var email = headers["X-User-Email"].FirstOrDefault();
        var primaryRole = headers["X-User-Role"].FirstOrDefault();
        var allRoles = headers["X-User-Roles"].FirstOrDefault();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };

        if (!string.IsNullOrEmpty(email))
            claims.Add(new Claim(ClaimTypes.Email, email));

        if (!string.IsNullOrEmpty(primaryRole))
            claims.Add(new Claim(ClaimTypes.Role, primaryRole));

        if (!string.IsNullOrEmpty(allRoles))
        {
            var roleList = allRoles.Split(',').Select(r => r.Trim()).Where(r => !string.IsNullOrEmpty(r));
            foreach (var role in roleList)
            {
                if (!claims.Any(c => c.Type == ClaimTypes.Role && c.Value == role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }
        }

        var identity = new ClaimsIdentity(claims, "ApiGateway");
        var principal = new ClaimsPrincipal(identity);

        context.HttpContext.User = principal;

        if (!string.IsNullOrEmpty(Roles))
        {
            var allowedRoles = Roles.Split(',').Select(r => r.Trim());
            var userRoles = context.HttpContext.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);

            if (!allowedRoles.Intersect(userRoles).Any())
            {
                context.Result = new ForbidResult();
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}