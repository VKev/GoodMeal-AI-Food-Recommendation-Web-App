using Application.Prompt.Commands;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedLibrary.Contracts.RatingPrompt;

namespace Application.Consumers;

public class ProcessRatingConsumer: IConsumer<ProcessRatingPromptEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessRatingConsumer> _logger;

    public ProcessRatingConsumer(IMediator mediator, ILogger<ProcessRatingConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProcessRatingPromptEvent> context)
    {
        try
        {
            var ratingByGeminiCommand = new RateByGeminiCommand(context.Message.Comment);
            var aiResult = await _mediator.Send(ratingByGeminiCommand, context.CancellationToken);
            _logger.LogInformation("AI Result: {@AiResult}", aiResult);
            // Publish kết quả về cho Saga
            await context.Publish(new PromptProcessedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                UserId = context.Message.UserId,
                RestaurantId = context.Message.RestaurantId,
                Comment = context.Message.Comment,
                AIScore = aiResult.Value,
                ImageUrl = context.Message.ImageUrl,
                ProcessedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            // Nếu lỗi, publish event thất bại
            await context.Publish(new PromptProcessFailedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                Reason = ex.Message,
                FailedAt = DateTime.UtcNow
            });
        }
    }
}