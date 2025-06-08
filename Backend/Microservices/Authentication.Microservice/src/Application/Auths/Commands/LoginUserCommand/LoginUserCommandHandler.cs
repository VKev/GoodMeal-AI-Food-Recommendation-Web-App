using Application.Abstractions.Messaging;
using Domain.Repositories;
using MassTransit;
using SharedLibrary.Common.ResponseModel;
using SharedLibrary.Contracts.UserCreating;

namespace Application.Auths.Commands;

public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IAuthRepository _authRepository;

    public LoginUserCommandHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<Result> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var jwt = await _authRepository.LoginAsync(request.Email, request.Password, cancellationToken);

        // await _publishEndpoint.Publish(new AuthenticationUserCreatingSagaStart()
        // {
        //     CorrelationId = Guid.NewGuid(),
        //     Email = request.Email,
        //     Name = request.Email,
        //     IdentityId = jwt.UserId,
        // }, cancellationToken);

        return Result.Success(jwt);
    }
}