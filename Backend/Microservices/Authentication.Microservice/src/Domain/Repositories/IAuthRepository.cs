using Domain.Entities;

namespace Domain.Repositories;

public interface IAuthRepository
{
    Task<string> RegisterAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<JwtResponse> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<JwtResponse> LoginWithExternalProviderAsync(string identityToken, CancellationToken cancellationToken = default);
}