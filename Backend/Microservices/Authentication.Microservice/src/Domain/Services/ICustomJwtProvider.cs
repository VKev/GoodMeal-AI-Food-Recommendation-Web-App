using Domain.Entities;

namespace Domain.Services;

public interface ICustomJwtProvider
{
    Task<JwtResponse> GenerateJwtWithRolesAsync(string identityId, string email, string name, CancellationToken cancellationToken = default);
} 