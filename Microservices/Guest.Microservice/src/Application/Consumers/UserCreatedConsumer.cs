using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.UnitOfWork;
using Application.Guests.Commands;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MassTransit;
using SharedLibrary.Contracts.UserCreating;

namespace Application.Consumers
{
    public class UserCreatedConsumer : IConsumer<UserCreatedEvent>
    {
        private readonly IGuestRepository _guestRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserCreatedConsumer(IGuestRepository guestRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _guestRepository = guestRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            try
            {
                var command = new CreateGuestCommand(context.Message.Name, context.Message.Email);
                await _guestRepository.AddAsync(_mapper.Map<Guest>(command), context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                await context.Publish(new GuestCreatedEvent
                {
                    CorrelationId = context.Message.CorrelationId
                });
            }catch(Exception ex){
                await context.Publish(new GuestCreatedFailureEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    Reason = ex.Message
                });
            }
        }
    }
}