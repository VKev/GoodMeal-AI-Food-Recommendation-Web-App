namespace SharedLibrary.Contracts.Authentication;

public class EnableUserRequest
{
    public string IdentityId { get; set; } = null!;
}

public class DisableUserRequest
{
    public string IdentityId { get; set; } = null!;
}

public class DeleteUserRequest
{
    public string IdentityId { get; set; } = null!;
}

public class UserCommandResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
    public string? ErrorCode { get; set; }
}

public class SearchUsersRequest
{
    public string? SearchTerm { get; set; }
    public int PageSize { get; set; } = 50;
    public string? PageToken { get; set; }
}

public class UserInfo
{
    public string Uid { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public bool IsDisabled { get; set; }
    public bool IsEmailVerified { get; set; }
    public DateTime? LastSignInTime { get; set; }
    public DateTime? CreationTime { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class SearchUsersResponse
{
    public IEnumerable<UserInfo> Users { get; set; } = new List<UserInfo>();
    public string? NextPageToken { get; set; }
    public int TotalCount { get; set; }
    public string SearchTerm { get; set; } = null!;
} 