using Application.Abstractions.Messaging;
using Domain.Repositories;
using MassTransit;
using SharedLibrary.Common.ResponseModel;
using SharedLibrary.Contracts.UserCreating;

namespace Application.Auths.Commands;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand>
{
    private IAuthRepository _authRepository;

    private readonly IPublishEndpoint _publishEndpoint;

    public RegisterUserCommandHandler(IAuthRepository authRepository, IPublishEndpoint publishEndpoint)
    {
        _authRepository = authRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var identityId = await _authRepository.RegisterAsync(request.Email, request.Password, cancellationToken);

        await _publishEndpoint.Publish(new AuthenticationUserCreatingSagaStart()
        {
            CorrelationId = Guid.NewGuid(),
            Email = request.Email,
            Name = request.Name,
            IdentityId = identityId,
        }, cancellationToken);

        return Result.Success();
    }
}