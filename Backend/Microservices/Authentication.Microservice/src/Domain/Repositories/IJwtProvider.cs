using SharedLibrary.Common.ResponseModel;

namespace Domain.Repositories;

public interface IJwtProvider
{
    Task<JwtResponse> GetForCredential(string email, string password, CancellationToken cancellationToken);
}