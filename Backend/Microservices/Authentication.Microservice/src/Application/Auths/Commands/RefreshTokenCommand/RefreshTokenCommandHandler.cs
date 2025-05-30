using Application.Abstractions.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.Auths.Commands;

public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand>
{
    public async Task<Result> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
} 