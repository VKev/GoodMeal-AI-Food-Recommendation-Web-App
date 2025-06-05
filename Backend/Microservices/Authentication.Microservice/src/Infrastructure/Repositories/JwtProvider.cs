using System.Net.Http.Json;
using Domain.Entities;
using Domain.Repositories;
using Newtonsoft.Json;

namespace Infrastructure.Repositories;

public class JwtProvider : IJwtProvider
{
    private readonly HttpClient _httpClient;

    public JwtProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<JwtResponse> GetForCredential(string email, string password, CancellationToken cancellationToken)
    {
        var request = new
        {
            email,
            password,
            returnSecureToken = true
        };
        
        var response = await _httpClient.PostAsJsonAsync("", request, cancellationToken: cancellationToken);
        var userResponse = await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: cancellationToken);
        return new JwtResponse()
        {
            IdentityId = userResponse?.LocalId ?? "",
            Email = email,
            Name = userResponse?.DisplayName ?? "",
            AccessToken = userResponse?.IdToken ?? "",
            RefreshToken = userResponse?.RefreshToken ?? "",
            TokenType = userResponse?.OauthAccessToken ?? "",
            ExpiresIn = userResponse?.OauthExpireIn ?? 0,
        };
    }
}

public class AuthResponse
{
    [JsonProperty("kind")] public string Kind { get; set; }

    [JsonProperty("localId")] public string LocalId { get; set; }

    [JsonProperty("email")] public string Email { get; set; }

    [JsonProperty("displayName")] public string DisplayName { get; set; }

    [JsonProperty("idToken")] public string IdToken { get; set; }

    [JsonProperty("registered")] public bool Registered { get; set; }

    [JsonProperty("profilePicture")] public string ProfilePicture { get; set; }

    [JsonProperty("oauthAccessToken")] public string OauthAccessToken { get; set; }

    [JsonProperty("oauthExpireIn")] public int OauthExpireIn { get; set; }

    [JsonProperty("oauthAuthorizationCode")]
    public string OauthAuthorizationCode { get; set; }

    [JsonProperty("refreshToken")] public string RefreshToken { get; set; }

    [JsonProperty("expiresIn")] public string ExpiresIn { get; set; }

    [JsonProperty("mfaPendingCredential")] public string MfaPendingCredential { get; set; }
}