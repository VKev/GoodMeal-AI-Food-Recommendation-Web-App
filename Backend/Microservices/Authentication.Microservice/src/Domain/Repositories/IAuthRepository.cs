namespace Domain.Repositories;

public interface IAuthRepository
{
    Task<string> RegisterAsync(string email, string password, string name, CancellationToken cancellationToken = default);
}