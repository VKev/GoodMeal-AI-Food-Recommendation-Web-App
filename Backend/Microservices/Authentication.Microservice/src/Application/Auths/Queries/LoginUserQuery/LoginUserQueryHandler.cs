using Application.Abstractions.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.Auths.Commands;

public class LoginUserQueryHandler : ICommandHandler<LoginUserQuery>
{
    public async Task<Result> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}