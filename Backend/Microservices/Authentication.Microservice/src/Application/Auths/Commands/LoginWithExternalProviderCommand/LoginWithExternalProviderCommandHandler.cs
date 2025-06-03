using Application.Abstractions.Messaging;
using Domain.Repositories;
using MassTransit;
using SharedLibrary.Common.ResponseModel;
using SharedLibrary.Contracts.UserCreating;

namespace Application.Auths.Commands;

public class LoginWithExternalProviderCommandHandler : ICommandHandler<LoginWithExternalProviderCommand>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IAuthRepository _authRepository;

    public LoginWithExternalProviderCommandHandler(IAuthRepository authRepository, IPublishEndpoint publishEndpoint)
    {
        _authRepository = authRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result> Handle(LoginWithExternalProviderCommand request, CancellationToken cancellationToken)
    {
        var jwt = await _authRepository.LoginWithExternalProviderAsync(request.IdentityToken,
            cancellationToken);

        await _publishEndpoint.Publish(new AuthenticationUserCreatingSagaStart()
        {
            CorrelationId = Guid.NewGuid(),
            Email = jwt.Email,
            Name = jwt.Name,
            IdentityId = jwt.UserId,
        }, cancellationToken);

        return Result.Success(jwt);
    }
}