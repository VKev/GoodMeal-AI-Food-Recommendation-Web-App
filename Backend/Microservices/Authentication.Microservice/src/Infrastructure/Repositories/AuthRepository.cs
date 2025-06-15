using Domain.Entities;
using Domain.Repositories;
using FirebaseAdmin.Auth;

namespace Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly IJwtProvider _jwtProvider;

    public AuthRepository(IJwtProvider jwtProvider)
    {
        _jwtProvider = jwtProvider;
    }

    public async Task<string> RegisterAsync(string email, string password,
        CancellationToken cancellationToken = default)
    {
        var userRecordArgs = new UserRecordArgs()
        {
            Email = email,
            Password = password
        };

        var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userRecordArgs, cancellationToken);

        return userRecord.Uid;
    }

    public async Task<JwtResponse> LoginAsync(string email, string password,
        CancellationToken cancellationToken = default)
    {
        var jwt = await _jwtProvider.GetForCredential(email, password, cancellationToken);
        return jwt;
    }

    public async Task<JwtResponse> LoginWithExternalProviderAsync(string identityToken,
        CancellationToken cancellationToken = default)
    {
        var decodedToken = await FirebaseAuth.DefaultInstance
            .VerifyIdTokenAsync(identityToken, cancellationToken);

        var uid = decodedToken.Uid;
        var email = decodedToken.Claims.TryGetValue("email", out var emailObj) ? emailObj?.ToString() : null;
        var name = decodedToken.Claims.TryGetValue("name", out var nameObj) ? nameObj?.ToString() : null;
        var picture = decodedToken.Claims.TryGetValue("picture", out var picObj) ? picObj?.ToString() : null;

        if (string.IsNullOrEmpty(email))
        {
            throw new Exception("Invalid token: email claim missing.");
        }

        return new JwtResponse
        {
            IdentityId = uid,
            Email = email,
            Name = name ?? "",
            AccessToken = identityToken,
            RefreshToken = "",
            TokenType = "Firebase",
            ExpiresIn = 3600,
        };
    }
}