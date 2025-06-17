using SharedLibrary.Common.ResponseModel;
using SharedLibrary.Common.Messaging;
using Application.Common;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using SharedLibrary.Contracts.UserCreating;
using SharedLibrary.Common;
using SharedLibrary.Common.Event;

namespace Application.Users.Commands
{
    public sealed record CreateUserCommand(
        string Name,
        string Email
    ) : ICommand;
    internal sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEventUnitOfWork _events;

        public CreateUserCommandHandler(
            IUserRepository userRepository, 
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            IEventUnitOfWork events)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _events = events;
        }

        public async Task<Result> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            await _userRepository.AddAsync(_mapper.Map<User>(command), cancellationToken);
            
            _events.Add(new UserCreatingSagaStart
            {
                CorrelationId = Guid.NewGuid(),
                Name = command.Name,
                Email = command.Email
            });

            return Result.Success();
        }
    }
}