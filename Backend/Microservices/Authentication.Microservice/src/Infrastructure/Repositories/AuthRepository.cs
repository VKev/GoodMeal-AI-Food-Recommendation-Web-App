using Domain.Repositories;
using FirebaseAdmin.Auth;
using SharedLibrary.Common.ResponseModel;

namespace Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    public async Task<string> RegisterAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var userRecordArgs = new UserRecordArgs()
        {
            Email = email,
            Password = password
        };

        var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userRecordArgs, cancellationToken);

        return userRecord.Uid;
    }

    public async Task<JwtResponse> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<JwtResponse> GenerateTokenAsync(object user, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<JwtResponse>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<JwtResponse>> LoginWithExternalProviderAsync(string provider, string providerKey, string identityToken,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}