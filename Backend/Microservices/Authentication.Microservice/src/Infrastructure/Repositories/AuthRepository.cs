using Domain.Repositories;
using FirebaseAdmin.Auth;
using SharedLibrary.Common.ResponseModel;

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

    public async Task<JwtResponse> GenerateTokenAsync(object user, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<JwtResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<JwtResponse> LoginWithExternalProviderAsync(string provider, string providerKey,
        string identityToken,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}