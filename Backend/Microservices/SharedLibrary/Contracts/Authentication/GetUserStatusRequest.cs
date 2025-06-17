namespace SharedLibrary.Contracts.Authentication;

public class GetUserStatusRequest
{
    public string IdentityId { get; set; } = null!;
}

public class GetUserStatusResponse
{
    public string IdentityId { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public bool IsDisabled { get; set; }
    public bool EmailVerified { get; set; }
    public DateTime? LastSignInTime { get; set; }
    public DateTime? CreationTime { get; set; }
} 