using Application.Abstractions.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.Auths.Commands;

public class LoginWithGitHubQueryHandler : ICommandHandler<LoginWithGitHubQuery>
{
    public async Task<Result> Handle(LoginWithGitHubQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}