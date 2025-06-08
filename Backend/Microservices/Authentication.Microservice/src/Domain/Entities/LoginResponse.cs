namespace Domain.Entities;

public class LoginResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string IdToken { get; set; } = string.Empty;
    public string IdentityId { get; set; } = string.Empty;
    public IEnumerable<string> Roles { get; set; } = new List<string>();
    public string PrimaryRole { get; set; } = "User";
    public long ExpiresIn { get; set; } = 3600;
}