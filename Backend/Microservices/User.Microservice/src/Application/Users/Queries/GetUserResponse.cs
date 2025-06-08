namespace Application.Users.Queries
{
    public sealed record GetUserResponse(
        string Name,
        string Email
    );
}