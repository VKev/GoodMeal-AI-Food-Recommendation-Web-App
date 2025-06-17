using Domain.Repositories;
using FirebaseAdmin.Auth;

namespace Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    public AuthRepository()
    {
    }

    public async Task<string> RegisterAsync(string email, string password,
        CancellationToken cancellationToken = default)
    {
        var userRecordArgs = new UserRecordArgs()
        {
            Email = email,
            Password = password
        };

        var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userRecordArgs, cancellationToken);

        return userRecord.Uid;
    }
}