using Application.Abstractions.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.Auths.Commands;

public class LoginWithExternalProviderCommandHandler : ICommandHandler<LoginWithExternalProviderCommand>
{
    public async Task<Result> Handle(LoginWithExternalProviderCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
} 