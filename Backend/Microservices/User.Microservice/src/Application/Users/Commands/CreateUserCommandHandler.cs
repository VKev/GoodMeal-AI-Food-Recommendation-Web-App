using System;
using System.Collections.Generic;
using SharedLibrary.Common.ResponseModel;
using Application.Abstractions.Messaging;
using Application.Abstractions.UnitOfWork;
using Application.Common;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MassTransit;
using MediatR;
using SharedLibrary.Contracts.UserCreating;

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

        private readonly IPublishEndpoint _publishEndpoint;

        public CreateUserCommandHandler(IUserRepository userRepository, IMapper mapper, IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint;
        }
        public async Task<Result> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            await _userRepository.AddAsync(_mapper.Map<User>(command), cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _publishEndpoint.Publish(new UserCreatingSagaStart{
                CorrelationId= Guid.NewGuid(),
                Name = command.Name,
                Email = command.Email
            }, cancellationToken);
            return Result.Success();
        }
    }
}