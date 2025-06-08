using MassTransit;
using SharedLibrary.Contracts.UserCreating;

namespace Application.Sagas
{
    public class AuthenticationUserCreatingSaga : MassTransitStateMachine<AuthenticationUserCreatingSagaData>
    {
        public State UserCreating { get; set; }
        public State Completed { get; set; }
        public State Failed { get; set; }
        
        public Event<AuthenticationUserCreatingSagaStart> authenticationUserCreated { get; set; }
        public Event<UserCreatedEvent> UserCreated { get; set; }
        public Event<UserCreatedFailureEvent> UserCreatedFailed { get; set; }

        public AuthenticationUserCreatingSaga()
        {
            InstanceState(x => x.CurrentState);

            Event(() => authenticationUserCreated, e => e.CorrelateById(m => m.Message.CorrelationId));
            Event(() => UserCreated, e => e.CorrelateById(m => m.Message.CorrelationId));
            Event(() => UserCreatedFailed, e => e.CorrelateById(m => m.Message.CorrelationId));

            Initially(
                When(authenticationUserCreated)
                .TransitionTo(UserCreating)
                .ThenAsync(async context =>
                {
                    context.Saga.CorrelationId = context.Message.CorrelationId;
                    context.Saga.UserCreated = true;

                    await context.Publish(new AuthenticationUserCreatedEvent
                    {
                        CorrelationId = context.Message.CorrelationId,
                        Name = context.Message.Name,
                        Email = context.Message.Email,
                        IdentityID = context.Message.IdentityId,
                    });
                })
            );

            During(UserCreating,
                When(UserCreated)
                    .Then(context =>
                    {
                        context.Saga.UserCreated = true;
                    })
                    .TransitionTo(Completed),

                When(UserCreatedFailed)
                    .Then(context =>
                    {
                        Console.WriteLine($"User creation failed: {context.Message.Reason}");
                    })
                    .TransitionTo(Failed)
            );

            SetCompletedWhenFinalized();
        }
    }
}