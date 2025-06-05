using System.Net.Http.Json;
using Domain.Entities;
using Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common.ResponseModel;

namespace Infrastructure.Services;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserService> _logger;
    private readonly string _userServiceBaseUrl;

    public UserService(HttpClient httpClient, IConfiguration configuration, ILogger<UserService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _userServiceBaseUrl = configuration["Services:UserService:BaseUrl"] ?? "http://api-gateway:8080";
    }

    public async Task<UserRolesResponse?> GetUserRolesAsync(string identityId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<Result<UserRolesResponse>>(
                $"{_userServiceBaseUrl}/api/User/roles/{identityId}",
                cancellationToken);

            if (response == null || response.IsFailure)
            {
                throw new Exception($"Failed to get user roles for identity {identityId}");
            }

            return response?.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting user roles for identity {IdentityId}", identityId);
            return null;
        }
    }
}