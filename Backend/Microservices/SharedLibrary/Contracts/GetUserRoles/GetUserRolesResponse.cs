namespace SharedLibrary.Contracts.GetUserRoles;

public record GetUserRolesResponse
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string IdentityId { get; init; } = string.Empty;
    public IEnumerable<string> Roles { get; init; } = new List<string>();
} 