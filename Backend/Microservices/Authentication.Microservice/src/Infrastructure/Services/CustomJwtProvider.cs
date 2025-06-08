using Domain.Services;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;
using Domain.Entities;

namespace Infrastructure.Services;

public class CustomJwtProvider : ICustomJwtProvider
{
    private readonly IUserService _userService;
    private readonly ILogger<CustomJwtProvider> _logger;

    public CustomJwtProvider(IUserService userService, ILogger<CustomJwtProvider> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task<JwtResponse> GenerateJwtWithRolesAsync(string identityId, string email, string name, CancellationToken cancellationToken = default)
    {
        try
        {
            var userRoles = await _userService.GetUserRolesAsync(identityId, cancellationToken);

            var claims = new Dictionary<string, object>
            {
                ["email"] = email,
                ["name"] = name,
                ["user_id"] = identityId
            };

            if (userRoles?.Roles?.Any() == true)
            {
                claims["roles"] = userRoles.Roles.ToArray();
                _logger.LogInformation("Added roles {Roles} to JWT for user {IdentityId}",
                    string.Join(", ", userRoles.Roles), identityId);
            }
            else
            {
                claims["roles"] = new[] { "User" };
                _logger.LogInformation("No roles found for user {IdentityId}, assigned default 'User' role", identityId);
            }

            var customToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(identityId, claims, cancellationToken);

            return new JwtResponse
            {
                IdentityId = identityId,
                Email = email,
                Name = name,
                AccessToken = customToken,
                RefreshToken = "",
                TokenType = "CustomFirebase",
                ExpiresIn = 3600,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating custom JWT for user {IdentityId}", identityId);

            var basicToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(identityId, new Dictionary<string, object>
            {
                ["email"] = email,
                ["name"] = name,
                ["user_id"] = identityId,
                ["roles"] = new[] { "User" }
            }, cancellationToken);

            return new JwtResponse
            {
                IdentityId = identityId,
                Email = email,
                Name = name,
                AccessToken = basicToken,
                RefreshToken = "",
                TokenType = "CustomFirebase",
                ExpiresIn = 3600,
            };
        }
    }
}