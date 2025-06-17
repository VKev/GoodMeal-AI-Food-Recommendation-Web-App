using MassTransit;
using SharedLibrary.Contracts.RestaurantCreating;

namespace Application.Sagas;

public class RestaurantCreatingSaga : MassTransitStateMachine<RestaurantCreatingSagaData>
{
    public State RestaurantCreating { get; set; }
    public State BusinessRestaurantCreating { get; set; }
    public State Completed { get; set; }
    public State Failed { get; set; }

    public Event<RestaurantCreatingSagaStart> RestaurantCreationStarted { get; set; }
    public Event<RestaurantCreatedEvent> RestaurantCreated { get; set; }
    public Event<RestaurantCreatedFailureEvent> RestaurantCreatedFailed { get; set; }
    public Event<BusinessRestaurantCreatedEvent> BusinessRestaurantCreated { get; set; }
    public Event<BusinessRestaurantCreatedFailureEvent> BusinessRestaurantCreatedFailed { get; set; }

    public RestaurantCreatingSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(() => RestaurantCreationStarted, e => e.CorrelateById(m => m.Message.CorrelationId));
        Event(() => RestaurantCreated, e => e.CorrelateById(m => m.Message.CorrelationId));
        Event(() => RestaurantCreatedFailed, e => e.CorrelateById(m => m.Message.CorrelationId));
        Event(() => BusinessRestaurantCreated, e => e.CorrelateById(m => m.Message.CorrelationId));
        Event(() => BusinessRestaurantCreatedFailed, e => e.CorrelateById(m => m.Message.CorrelationId));

        Initially(
            When(RestaurantCreationStarted)
                .TransitionTo(RestaurantCreating)
                .ThenAsync(async context =>
                {
                    context.Saga.CorrelationId = context.Message.CorrelationId;
                    context.Saga.BusinessId = context.Message.BusinessId;
                    context.Saga.RestaurantId = context.Message.RestaurantId;
                    context.Saga.RestaurantName = context.Message.Name;
                    context.Saga.CreatedBy = context.Message.CreatedBy;

                    // Publish event to create restaurant in Restaurant microservice
                    await context.Publish(new CreateRestaurantEvent
                    {
                        CorrelationId = context.Message.CorrelationId,
                        RestaurantId = context.Message.RestaurantId,
                        Name = context.Message.Name,
                        Address = context.Message.Address,
                        Phone = context.Message.Phone,
                        CreatedBy = context.Message.CreatedBy
                    });
                })
        );

        During(RestaurantCreating,
            When(RestaurantCreated)
                .TransitionTo(BusinessRestaurantCreating)
                .ThenAsync(async context =>
                {
                    context.Saga.RestaurantCreated = true;

                    // Publish event to create BusinessRestaurant relationship
                    await context.Publish(new CreateBusinessRestaurantEvent
                    {
                        CorrelationId = context.Message.CorrelationId,
                        BusinessId = context.Saga.BusinessId,
                        RestaurantId = context.Message.RestaurantId,
                        CreatedBy = context.Saga.CreatedBy
                    });
                }),

            When(RestaurantCreatedFailed)
                .Then(context =>
                {
                    context.Saga.FailureReason = context.Message.Reason;
                    Console.WriteLine($"Restaurant creation failed: {context.Message.Reason}");
                })
                .TransitionTo(Failed)
        );

        During(BusinessRestaurantCreating,
            When(BusinessRestaurantCreated)
                .Then(context =>
                {
                    context.Saga.BusinessRestaurantCreated = true;
                    Console.WriteLine($"Restaurant {context.Saga.RestaurantName} successfully added to business {context.Saga.BusinessId}");
                })
                .TransitionTo(Completed),

            When(BusinessRestaurantCreatedFailed)
                .Then(context =>
                {
                    context.Saga.FailureReason = context.Message.Reason;
                    Console.WriteLine($"BusinessRestaurant creation failed: {context.Message.Reason}");
                })
                .TransitionTo(Failed)
        );

        SetCompletedWhenFinalized();
    }
} 