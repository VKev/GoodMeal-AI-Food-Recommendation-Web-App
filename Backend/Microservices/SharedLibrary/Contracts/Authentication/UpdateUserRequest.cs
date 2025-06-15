namespace SharedLibrary.Contracts.Authentication;

public class UpdateUserRequest
{
    public string IdentityId { get; set; } = null!;
    public string? Email { get; set; }  
    public string? DisplayName { get; set; }
    public bool? EmailVerified { get; set; }
}

public class UpdateUserResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
} 