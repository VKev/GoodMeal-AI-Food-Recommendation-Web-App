using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.GeminiApi;
using Application.Prompt.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Common;
using SharedLibrary.Common.Messaging.Commands;
using SharedLibrary.Common.ResponseModel;
using SharedLibrary.Utils.AuthenticationExtention;

namespace WebApi.Controllers;

[ApiController]
[Route("api/prompt/[controller]")]
public class GeminiController : ApiController
{
    public GeminiController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost("send-message")]
    [ApiGatewayUser]
    public async Task<IActionResult> SendMessage([FromBody] CreateMessageCommand request,
        CancellationToken cancellationToken)
    {
        var geminiResult = await _mediator.Send(new CallGeminiCommand(request.PromptMessage), cancellationToken);
        var createMessageResult =
            await _mediator.Send(request with { ResponseMessage = geminiResult.Value }, cancellationToken);

        var aggregatedResult = ResultAggregator.Aggregate(geminiResult, createMessageResult);
        if (aggregatedResult.IsFailure)
        {
            return HandleFailure(aggregatedResult);
        }

        return Ok(aggregatedResult);
    }

    [HttpPost("search-images")]
    [ApiGatewayUser]
    public async Task<IActionResult> SearchImagesAsync(
        [FromBody] GoogleImageSearchCommand request,
        CancellationToken cancellationToken)
    {
        if (!request.FoodNames.Any())
        {
            var validationResult = Result.Failure(Error.NullValue);
            return HandleFailure(validationResult);
        }

        var searchResult = await _mediator.Send(request, cancellationToken);

        var aggregatedResult = ResultAggregator.Aggregate(searchResult);
        if (aggregatedResult.IsFailure)
        {
            return HandleFailure(aggregatedResult);
        }

        var orderedImages = request.FoodNames
            .Select(foodName => new
            {
                FoodName = foodName,
                ImageUrl = searchResult.Value.ContainsKey(foodName)
                    ? searchResult.Value[foodName]
                    : null
            })
            .ToList();

        return Ok(orderedImages);
    }

    [HttpPost("rate-comment")]
    [ApiGatewayUser]
    public async Task<IActionResult> RateCommentAsync([FromBody] RateByGeminiCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RateByGeminiCommand(request.UserComment), cancellationToken);

        var aggregateResult = ResultAggregator.AggregateWithNumbers(
            (result, true));
        if (aggregateResult.IsFailure)
        {
            return HandleFailure(aggregateResult);
        }

        return Ok(aggregateResult);
    }


    [HttpPost("process-food-request")]
    [ApiGatewayUser]
    public async Task<IActionResult> ProcessFoodRequestAsync(
        [FromBody] CreateMessageCommand request,
        CancellationToken cancellationToken)
    {
        var geminiResult = await _mediator.Send(new CallGeminiCommand(request.PromptMessage), cancellationToken);

        var cleanedJson = CleanGeminiResponse.CleanResponse(geminiResult.Value);

        var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(
            cleanedJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });


        Result<Dictionary<string, string>> imageSearchResult =
            Result.Failure<Dictionary<string, string>>(Error.NullValue);

        imageSearchResult =
            await _mediator.Send(new GoogleImageSearchCommand(geminiResponse.FoodNames), cancellationToken);


        foreach (var food in geminiResponse.Foods)
        {
            if (imageSearchResult.Value.TryGetValue(food.FoodName, out var imageUrl))
            {
                food.ImageUrl = imageUrl;
            }
        }

        var finalResponseJson = JsonSerializer.Serialize(geminiResponse);

        var saveResult = await _mediator.Send(
            request with { ResponseMessage = finalResponseJson }, cancellationToken);

        var save = await _mediator.Send(new SaveChangesCommand(), cancellationToken);

        var aggregateResult = ResultAggregator.AggregateWithNumbers(
            (geminiResult, false),
            (imageSearchResult, false),
            (saveResult, true), (save, false)
        );

        if (aggregateResult.IsFailure)
            return HandleFailure(aggregateResult);

        return Ok(new
        {
            AggregatedResults = aggregateResult.Value,
            FinalResponse = geminiResponse
        });
    }
}