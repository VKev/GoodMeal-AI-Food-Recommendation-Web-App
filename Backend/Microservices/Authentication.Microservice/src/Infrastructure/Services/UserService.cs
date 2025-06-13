using Domain.Entities;
using Domain.Services;
using Microsoft.Extensions.Logging;
using MassTransit;
using SharedLibrary.Contracts.GetUserRoles;

namespace Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IBus _bus;
    private readonly ILogger<UserService> _logger;

    public UserService(IBus bus, ILogger<UserService> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task<UserRolesResponse?> GetUserRolesAsync(string identityId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new GetUserRolesRequest { IdentityId = identityId };
            
            var requestClient = _bus.CreateRequestClient<GetUserRolesRequest>();
            var response = await requestClient.GetResponse<GetUserRolesResponse>(request, cancellationToken);
            
            var userRoles = response.Message;
            
            return new UserRolesResponse
            {
                UserId = userRoles.UserId,
                Email = userRoles.Email,
                Name = userRoles.Name,
                IdentityId = userRoles.IdentityId,
                Roles = userRoles.Roles
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting user roles for identity {IdentityId}", identityId);
            return null;
        }
    }
}