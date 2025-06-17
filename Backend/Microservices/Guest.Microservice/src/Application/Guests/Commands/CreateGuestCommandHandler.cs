using System;
using System.Collections.Generic;
using SharedLibrary.Common.ResponseModel;
using SharedLibrary.Common.Messaging;
using Application.Common;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using SharedLibrary.Common;

namespace Application.Guests.Commands
{
    public sealed record CreateGuestCommand(
        string Fullname,
        string Email
    ) : ICommand;
    internal sealed class CreateGuestCommandHandler : ICommandHandler<CreateGuestCommand>
    {
        private readonly IGuestRepository _guestRepository;
        private readonly IMapper _mapper;

        public CreateGuestCommandHandler(IGuestRepository guestRepository, IMapper mapper)
        {
            _guestRepository = guestRepository;
            _mapper = mapper;
        }
        
        public async Task<Result> Handle(CreateGuestCommand command, CancellationToken cancellationToken)
        {
            await _guestRepository.AddAsync(_mapper.Map<Guest>(command), cancellationToken);
            return Result.Success();
        }
    }
}