using SharedLibrary.Common.ResponseModel;

namespace Domain.Repositories;

public interface IAuthRepository
{
    Task<string> RegisterAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<JwtResponse> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    // Task<JwtResponse> GenerateTokenAsync(object user, CancellationToken cancellationToken = default);
    // Task<JwtResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    // Task<JwtResponse> LoginWithExternalProviderAsync(string provider, string providerKey, string identityToken, CancellationToken cancellationToken = default);
}