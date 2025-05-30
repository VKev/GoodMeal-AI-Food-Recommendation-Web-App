using Application.Abstractions.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.Auths.Commands;

public class LoginWithFacebookQueryHandler : ICommandHandler<LoginWithFacebookQuery>
{
    public async Task<Result> Handle(LoginWithFacebookQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
} 