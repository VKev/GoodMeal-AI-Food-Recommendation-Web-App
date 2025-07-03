using Domain.Repositories;
using FirebaseAdmin.Auth;

namespace Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    public AuthRepository()
    {
    }

    public async Task<string> RegisterAsync(string email, string password, string name,
        CancellationToken cancellationToken = default)
    {
        var userRecordArgs = new UserRecordArgs()
        {
            Email = email,
            Password = password,
            DisplayName = name
        };

        var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userRecordArgs, cancellationToken);

        var customClaims = new Dictionary<string, object>
        {
            ["roles"] = new[] { "User" },
            ["created_at"] = DateTime.UtcNow
        };

        await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userRecord.Uid, customClaims, cancellationToken);

        return userRecord.Uid;
    }
}