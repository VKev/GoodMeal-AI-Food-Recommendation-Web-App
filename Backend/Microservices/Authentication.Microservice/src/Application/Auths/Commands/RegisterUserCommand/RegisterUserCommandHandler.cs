using Domain.Repositories;
using FluentValidation;
using MassTransit;
using SharedLibrary.Common.Messaging;
using SharedLibrary.Common.ResponseModel;

namespace Application.Auths.Commands.RegisterUserCommand;
public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string Name
) : ICommand;

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
        await _authRepository.RegisterAsync(request.Email, request.Password, request.Name, cancellationToken);

        return Result.Success();
    }
}

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().MaximumLength(70).EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(70);
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(70);
    }
}