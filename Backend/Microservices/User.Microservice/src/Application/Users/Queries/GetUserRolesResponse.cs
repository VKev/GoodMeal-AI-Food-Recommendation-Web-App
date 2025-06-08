namespace Application.Users.Queries
{
    public sealed record GetUserRolesResponse(
        Guid UserId,
        string Email,
        string Name,
        string IdentityId,
        IEnumerable<string> Roles
    );
} 