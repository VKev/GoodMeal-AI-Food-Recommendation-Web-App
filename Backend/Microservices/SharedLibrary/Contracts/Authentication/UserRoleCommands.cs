namespace SharedLibrary.Contracts.Authentication;

// Get User Roles Request
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

// Add User Role Request
public class AddUserRoleRequest
{
    public string IdentityId { get; set; } = null!;
    public string RoleName { get; set; } = null!;
}

// Remove User Role Request  
public class RemoveUserRoleRequest
{
    public string IdentityId { get; set; } = null!;
    public string RoleName { get; set; } = null!;
}

// Role Command Response
public class RoleCommandResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
    public string? ErrorCode { get; set; }
} 