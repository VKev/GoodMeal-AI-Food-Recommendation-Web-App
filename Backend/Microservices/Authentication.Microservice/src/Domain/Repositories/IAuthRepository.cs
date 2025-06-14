using Domain.Entities;

namespace Domain.Repositories;

public interface IAuthRepository
{
    Task<string> RegisterAsync(string email, string password, CancellationToken cancellationToken = default);
}