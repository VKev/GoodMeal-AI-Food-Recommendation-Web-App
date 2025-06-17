namespace SharedLibrary.Contracts.Authentication;

public class GetUserRolesRequest
{
    public string IdentityId { get; set; } = null!;
}

public class GetUserRolesResponse
{
    public string IdentityId { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}

public class AddUserRoleRequest
{
    public string IdentityId { get; set; } = null!;
    public string RoleName { get; set; } = null!;
}

public class RemoveUserRoleRequest
{
    public string IdentityId { get; set; } = null!;
    public string RoleName { get; set; } = null!;
}

public class RoleCommandResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
    public string? ErrorCode { get; set; }
} 