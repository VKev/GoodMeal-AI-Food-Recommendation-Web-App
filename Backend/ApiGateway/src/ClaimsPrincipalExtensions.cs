using System.Security.Claims;

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
}
