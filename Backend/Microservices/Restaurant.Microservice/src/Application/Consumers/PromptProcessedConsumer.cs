using Application.Foods.Commands;
using Application.RestaurantRatings.Commands;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedLibrary.Common;
using SharedLibrary.Common.Messaging.Commands;
using SharedLibrary.Contracts.RatingPrompt;

namespace Application.Consumers;

public class PromptProcessedConsumer: IConsumer<RatingScoreReadyEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<RatingScoreReadyEvent> _logger;

    public PromptProcessedConsumer(IMediator mediator, ILogger<RatingScoreReadyEvent> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<RatingScoreReadyEvent> context)
    {
            var ratingByGeminiCommand = new CreateRatingCommand(context.Message.RestaurantId, context.Message.UserId,context.Message.Comment, context.Message.AIScore, context.Message.ImageUrl);
            var aiResult = await _mediator.Send(ratingByGeminiCommand, context.CancellationToken);
            _logger.LogInformation(aiResult.ToString());
            var saveResult = await _mediator.Send(new SaveChangesCommand(), context.CancellationToken);
            var aggregatedResult = ResultAggregator.AggregateWithNumbers(
                (aiResult, true), 
                (saveResult, false));
        
    }
}