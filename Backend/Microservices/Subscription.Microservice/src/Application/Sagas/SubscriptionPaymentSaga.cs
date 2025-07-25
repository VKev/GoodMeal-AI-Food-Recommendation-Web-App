using MassTransit;
using SharedLibrary.Contracts.SubscriptionPayment;

namespace Application.Sagas;

public class SubscriptionPaymentSaga : MassTransitStateMachine<SubscriptionPaymentSagaData>
{
    public State PaymentUrlCreating { get; set; }
    public State PaymentPending { get; set; }
    public State SubscriptionActivating { get; set; }
    public State Completed { get; set; }
    public State Failed { get; set; }

    public Event<SubscriptionPaymentSagaStart> PaymentRequested { get; set; }
    public Event<SubscriptionPaymentUrlCreatedEvent> PaymentUrlCreated { get; set; }
    public Event<SubscriptionPaymentUrlCreationFailedEvent> PaymentUrlCreationFailed { get; set; }
    public Event<SubscriptionPaymentCompletedEvent> PaymentCompleted { get; set; }
    public Event<SubscriptionPaymentFailedEvent> PaymentFailed { get; set; }
    public Event<UserSubscriptionActivatedEvent> SubscriptionActivated { get; set; }
    public Event<UserSubscriptionActivationFailedEvent> SubscriptionActivationFailed { get; set; }

    public SubscriptionPaymentSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(() => PaymentRequested, e => e.CorrelateById(m => m.Message.CorrelationId));
        Event(() => PaymentUrlCreated, e => e.CorrelateById(m => m.Message.CorrelationId));
        Event(() => PaymentUrlCreationFailed, e => e.CorrelateById(m => m.Message.CorrelationId));
        Event(() => PaymentCompleted, e => e.CorrelateById(m => m.Message.CorrelationId));
        Event(() => PaymentFailed, e => e.CorrelateById(m => m.Message.CorrelationId));
        Event(() => SubscriptionActivated, e => e.CorrelateById(m => m.Message.CorrelationId));
        Event(() => SubscriptionActivationFailed, e => e.CorrelateById(m => m.Message.CorrelationId));

        Initially(
            When(PaymentRequested)
                .TransitionTo(PaymentUrlCreating)
                .ThenAsync(async context =>
                {
                    context.Saga.CorrelationId = context.Message.CorrelationId;
                    context.Saga.UserId = context.Message.UserId;
                    context.Saga.SubscriptionId = context.Message.SubscriptionId;
                    context.Saga.Amount = context.Message.Amount;
                    context.Saga.Currency = context.Message.Currency;
                    context.Saga.RequestedAt = context.Message.RequestedAt;
                    context.Saga.OrderId = $"SUB_{context.Message.CorrelationId:N}";

                    // Publish event to create payment URL in Payment microservice
                    await context.Publish(new CreateSubscriptionPaymentUrlEvent
                    {
                        CorrelationId = context.Message.CorrelationId,
                        UserId = context.Message.UserId,
                        SubscriptionId = context.Message.SubscriptionId,
                        Amount = context.Message.Amount,
                        Currency = context.Message.Currency,
                        OrderDescription = context.Message.OrderDescription,
                        OrderId = context.Saga.OrderId,
                        IpAddress = context.Message.IpAddress
                    });
                })
        );

        During(PaymentUrlCreating,
            When(PaymentUrlCreated)
                .TransitionTo(PaymentPending)
                .Then(context =>
                {
                    context.Saga.PaymentUrl = context.Message.PaymentUrl;
                    context.Saga.PaymentUrlCreated = true;
                    context.Saga.TransactionId = context.Message.UrlCreatedAt;
                    Console.WriteLine($"Payment URL created for subscription {context.Saga.SubscriptionId}: {context.Message.PaymentUrl}");
                }),

            When(PaymentUrlCreationFailed)
                .Then(context =>
                {
                    context.Saga.FailureReason = context.Message.Reason;
                    Console.WriteLine($"Payment URL creation failed for subscription {context.Saga.SubscriptionId}: {context.Message.Reason}");
                })
                .TransitionTo(Failed)
        );

        During(PaymentPending,
            When(PaymentCompleted)
                .TransitionTo(SubscriptionActivating)
                .ThenAsync(async context =>
                {
                    context.Saga.PaymentCompleted = true;
                    context.Saga.TransactionId = context.Message.TransactionId;
                    context.Saga.CompletedAt = context.Message.CompletedAt;

                    // Publish event to activate user subscription using saga data
                    await context.Publish(new ActivateUserSubscriptionEvent
                    {
                        CorrelationId = context.Saga.CorrelationId,
                        UserId = context.Saga.UserId,
                        SubscriptionId = context.Saga.SubscriptionId,
                        OrderId = context.Saga.OrderId,
                        ActivatedAt = context.Message.CompletedAt
                    });
                }),

            When(PaymentFailed)
                .Then(context =>
                {
                    context.Saga.FailureReason = context.Message.Reason;
                    context.Saga.FailedAt = context.Message.FailedAt;
                    Console.WriteLine($"Payment failed for subscription {context.Saga.SubscriptionId}: {context.Message.Reason}");
                })
                .TransitionTo(Failed)
        );

        During(SubscriptionActivating,
            When(SubscriptionActivated)
                .Then(context =>
                {
                    context.Saga.SubscriptionActivated = true;
                    context.Saga.UserSubscriptionId = context.Message.UserSubscriptionId;
                    context.Saga.StartDate = context.Message.StartDate;
                    context.Saga.EndDate = context.Message.EndDate;
                    Console.WriteLine($"Subscription {context.Saga.SubscriptionId} successfully activated for user {context.Saga.UserId}");
                })
                .TransitionTo(Completed),

            When(SubscriptionActivationFailed)
                .Then(context =>
                {
                    context.Saga.FailureReason = context.Message.Reason;
                    Console.WriteLine($"Subscription activation failed for user {context.Saga.UserId}: {context.Message.Reason}");
                })
                .TransitionTo(Failed)
        );

        SetCompletedWhenFinalized();
    }
} 