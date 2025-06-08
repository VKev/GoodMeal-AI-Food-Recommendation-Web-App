using System.Text.Json.Serialization;

namespace Domain.Entities;

public class UserRolesResponse
{
    [JsonPropertyName("userId")]
    public Guid UserId { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("identityId")]
    public string IdentityId { get; set; } = string.Empty;

    [JsonPropertyName("roles")]
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}