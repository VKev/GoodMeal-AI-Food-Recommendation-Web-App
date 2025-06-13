using Application.Guests.Commands;
using MassTransit;
using MediatR;
using SharedLibrary.Common.Messaging.Commands;
using SharedLibrary.Contracts.UserCreating;

namespace WebApi.Consumers
{
    public class UserCreatedConsumer : IConsumer<UserCreatedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserCreatedConsumer> _logger;

        public UserCreatedConsumer(IMediator mediator, ILogger<UserCreatedConsumer> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            _logger.LogInformation("UserCreatedConsumer: Received UserCreatedEvent for user {Email} with CorrelationId {CorrelationId}", 
                context.Message.Email, context.Message.CorrelationId);
            
            try
            {
                // Use MediatR to send the create guest command
                var createGuestCommand = new CreateGuestCommand(context.Message.Name, context.Message.Email);
                var createResult = await _mediator.Send(createGuestCommand, context.CancellationToken);
                
                if (createResult.IsFailure)
                {
                    await context.Publish(new GuestCreatedFailureEvent
                    {
                        CorrelationId = context.Message.CorrelationId,
                        Reason = createResult.Error.Description
                    });
                    return;
                }

                // Use MediatR to save changes
                var saveResult = await _mediator.Send(new SaveChangesCommand(), context.CancellationToken);
                
                if (saveResult.IsFailure)
                {
                    await context.Publish(new GuestCreatedFailureEvent
                    {
                        CorrelationId = context.Message.CorrelationId,
                        Reason = saveResult.Error.Description
                    });
                    return;
                }

                // Publish success event
                await context.Publish(new GuestCreatedEvent
                {
                    CorrelationId = context.Message.CorrelationId
                });

                _logger.LogInformation("Guest created successfully");
            }
            catch (Exception ex)
            {
                await context.Publish(new GuestCreatedFailureEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    Reason = ex.Message
                });
            }
        }
    }
} 