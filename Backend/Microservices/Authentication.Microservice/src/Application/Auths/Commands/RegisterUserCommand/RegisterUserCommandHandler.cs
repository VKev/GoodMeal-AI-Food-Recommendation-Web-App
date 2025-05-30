using Application.Abstractions.Messaging;
using Application.Abstractions.UnitOfWork;
using AutoMapper;
using Domain.Repositories;
using MassTransit;
using SharedLibrary.Common.ResponseModel;
using SharedLibrary.Contracts.UserCreating;

namespace Application.Auths.Commands;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand>
{
    private IAuthRepository _authRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    private readonly IPublishEndpoint _publishEndpoint;

    public RegisterUserCommandHandler(IAuthRepository authRepository,
        IUnitOfWork unitOfWork, IMapper mapper, IPublishEndpoint publishEndpoint)
    {
        _authRepository = authRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var identityId = await _authRepository.RegisterAsync(request.Email, request.Password, cancellationToken);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
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