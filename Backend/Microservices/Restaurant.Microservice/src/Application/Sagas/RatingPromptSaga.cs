using MassTransit;
using SharedLibrary.Contracts.RatingPrompt;

namespace Application.Sagas;

 public class RatingPromptSaga : MassTransitStateMachine<RatingPromptSagaData>
    {
        public State PromptProcessing { get; set; }
        public State Completed { get; set; }
        public State Failed { get; set; }

        public Event<RatingCreatedSagaStart> RatingCreated { get; set; }
        public Event<PromptProcessedEvent> PromptProcessed { get; set; }
        public Event<PromptProcessFailedEvent> PromptProcessFailed { get; set; }

        public RatingPromptSaga()
        {
            InstanceState(x => x.CurrentState);

            Event(() => RatingCreated, e => e.CorrelateById(m => m.Message.CorrelationId));
            Event(() => PromptProcessed, e => e.CorrelateById(m => m.Message.CorrelationId));
            Event(() => PromptProcessFailed, e => e.CorrelateById(m => m.Message.CorrelationId));

            Initially(
                When(RatingCreated)
                    .TransitionTo(PromptProcessing)
                    .ThenAsync(async context =>
                    {
                        context.Saga.RatingId = context.Message.RatingId;
                        context.Saga.UserId = context.Message.UserId;
                        context.Saga.RestaurantId = context.Message.RestaurantId;
                        context.Saga.Comment = context.Message.Comment;
                        // Gửi event sang Prompt Service để AI xử lý
                        await context.Publish(new ProcessRatingPromptEvent
                        {
                            CorrelationId = context.Message.CorrelationId,
                            RatingId = context.Message.RatingId,
                            Comment = context.Message.Comment,
                            UserId = context.Message.UserId,
                            RestaurantId = context.Message.RestaurantId
                        });
                    })
            );

            During(PromptProcessing,
                When(PromptProcessed)
                    .Then(context =>
                    {
                        context.Saga.AIScore = context.Message.AIScore;
                    })
                    .TransitionTo(Completed),

                When(PromptProcessFailed)
                    .Then(context =>
                    {
                        context.Saga.FailureReason = context.Message.Reason;
                    })
                    .TransitionTo(Failed)
            );

            SetCompletedWhenFinalized();
        }
    }