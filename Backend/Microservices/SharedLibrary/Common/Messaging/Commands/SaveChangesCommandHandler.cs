using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MassTransit;
using SharedLibrary.Common;
using SharedLibrary.Common.ResponseModel;
using SharedLibrary.Common.Event;

namespace SharedLibrary.Common.Messaging.Commands
{
    public sealed record SaveChangesCommand : ICommand<int>;
    public class SaveChangesCommandHandler : ICommandHandler<SaveChangesCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IEventFlusher _eventFlusher;

        public SaveChangesCommandHandler(
            IUnitOfWork unitOfWork, 
            IPublishEndpoint publishEndpoint, 
            IEventFlusher eventFlusher)
        {
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint;
            _eventFlusher = eventFlusher;
        }

        public async Task<Result<int>> Handle(SaveChangesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
                
                await _eventFlusher.FlushAsync(_publishEndpoint, cancellationToken);
                
                return Result.Success(result);
            }
            catch (Exception ex)
            {
                return Result.Failure<int>(Error.FromException(ex));
            }
        }
    }
} 