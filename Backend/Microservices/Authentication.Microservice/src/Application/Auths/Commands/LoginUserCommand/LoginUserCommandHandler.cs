using Application.Abstractions.Messaging;
using Domain.Repositories;
using SharedLibrary.Common.ResponseModel;

namespace Application.Auths.Commands;

public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand>
{
    private readonly IAuthRepository _authRepository;

    public LoginUserCommandHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<Result> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var jwt = await _authRepository.LoginAsync(request.Email, request.Password, cancellationToken);
        return Result.Success(jwt);
    }
}