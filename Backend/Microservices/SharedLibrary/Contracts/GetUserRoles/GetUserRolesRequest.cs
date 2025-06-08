namespace SharedLibrary.Contracts.GetUserRoles;

public record GetUserRolesRequest
{
    public string IdentityId { get; init; } = string.Empty;
} 