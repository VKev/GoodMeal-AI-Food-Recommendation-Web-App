using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Helper;

public static class CheckUserId
{
    public static string GetUserId(HttpContext httpContext)
    {
        var userIdString = httpContext.User.FindFirst("user_id")?.Value;

        if (string.IsNullOrEmpty(userIdString))
            throw new UnauthorizedAccessException("Invalid or missing UserId in token claims.");

        return userIdString;
    }
}