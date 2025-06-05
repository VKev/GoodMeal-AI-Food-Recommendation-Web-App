using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace src;

public static class ClaimsPrincipalExtensions
{
    public static string GetFirebaseUserId(this ClaimsPrincipal user)
        => user.FindFirst("user_id")?.Value;

    public static string GetEmail(this ClaimsPrincipal user)
        => user.FindFirst(ClaimTypes.Email)?.Value ?? user.FindFirst("email")?.Value;

    public static bool IsEmailVerified(this ClaimsPrincipal user)
        => bool.TryParse(user.FindFirst("email_verified")?.Value, out var result) && result;

    public static string GetSignInProvider(this ClaimsPrincipal user)
        => user.FindFirst("firebase/sign_in_provider")?.Value;

    public static IEnumerable<string> GetRoles(this ClaimsPrincipal user)
    {
        var rolesClaim = user.FindFirst(ClaimTypes.Role)?.Value;
        if (string.IsNullOrEmpty(rolesClaim))
            return new List<string>();

        try
        {
            if (rolesClaim.StartsWith("[") && rolesClaim.EndsWith("]"))
            {
                var rolesArray = JsonSerializer.Deserialize<string[]>(rolesClaim);
                return rolesArray ?? new string[0];
            }

            return rolesClaim.Split(',').Select(r => r.Trim());
        }
        catch
        {
            return new List<string> { rolesClaim };
        }
    }

    public static string GetPrimaryRole(this ClaimsPrincipal user)
    {
        var roles = user.GetRoles();
        return roles.FirstOrDefault() ?? "User";
    }
}
