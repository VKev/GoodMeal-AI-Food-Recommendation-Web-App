using Application.Abstractions.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.Auths.Commands;

public class LoginWithGoogleQueryHandler : ICommandHandler<LoginWithGoogleQuery>
{
    public async Task<Result> Handle(LoginWithGoogleQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
} 