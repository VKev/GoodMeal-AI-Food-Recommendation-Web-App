using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MassTransit;
using SharedLibrary.Common;
using SharedLibrary.Contracts.UserCreating;

namespace Application.Consumers;

public class AuthenticationUserCreatedConsumer : IConsumer<AuthenticationUserCreatedEvent>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AuthenticationUserCreatedConsumer(IUserRepository userRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<AuthenticationUserCreatedEvent> context)
    {
        try
        {
            var user = new User
            {
                Name = context.Message.Name, 
                Email = context.Message.Email, 
                IdentityId = context.Message.IdentityID
            };
            
            await _userRepository.AddAsync(user, context.CancellationToken);
            await _unitOfWork.SaveChangesAsync(context.CancellationToken);
            
            await context.Publish(new UserCreatedEvent()
            {
                CorrelationId = context.Message.CorrelationId,
                Name = user.Name,
                Email = user.Email,
                IdentityId = user.IdentityId,
                UserId = user.UserId
            });
        }
        catch (Exception ex)
        {
            await context.Publish(new UserCreatedFailureEvent()
            {
                CorrelationId = context.Message.CorrelationId,
                Reason = ex.Message
            });
        }
    }
}