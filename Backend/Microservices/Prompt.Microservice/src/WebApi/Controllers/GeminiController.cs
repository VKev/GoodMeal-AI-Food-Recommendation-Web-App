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
using SharedLibrary.Common.ResponseModel;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeminiController : ApiController
{
    public GeminiController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost("send-message")]
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

        return Ok(geminiResult);
    }

    [HttpPost("search-images")]
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
    public async Task<IActionResult> RateCommentAsync([FromBody] RateByGeminiCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserComment))
            return BadRequest(Result.Failure(Error.NullValue));

        var result = await _mediator.Send(new RateByGeminiCommand(request.UserComment), cancellationToken);

        if (result.IsFailure)
            return HandleFailure(result);

        return Ok(new
        {
            Comment = request.UserComment,
            Stars = result.Value
        });
    }


    [HttpPost("process-food-request")]
    public async Task<IActionResult> ProcessFoodRequestAsync(
        [FromBody] CreateMessageCommand request,
        CancellationToken cancellationToken)
    {
        var geminiResult = await _mediator.Send(new CallGeminiCommand(request.PromptMessage), cancellationToken);

        GeminiResponse? geminiResponse = null;
        if (geminiResult.IsSuccess)
        {
            var cleanedJson = CleanGeminiResponse.CleanResponse(geminiResult.Value);

            geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(
                cleanedJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (geminiResponse == null || !geminiResponse.FoodNames.Any())
            {
                geminiResponse = null;
            }
        }

        Result<Dictionary<string, string>> imageSearchResult =
            Result.Failure<Dictionary<string, string>>(Error.NullValue);
        if (geminiResponse != null)
        {
            imageSearchResult =
                await _mediator.Send(new GoogleImageSearchCommand(geminiResponse.FoodNames), cancellationToken);

            if (imageSearchResult.IsSuccess)
            {
                foreach (var food in geminiResponse.Foods)
                {
                    if (imageSearchResult.Value.TryGetValue(food.FoodName, out var imageUrl))
                    {
                        food.ImageUrl = imageUrl;
                    }
                }
            }
        }

        Result saveResult = Result.Failure(Error.NullValue);
        if (geminiResponse != null)
        {
            var finalResponseJson = JsonSerializer.Serialize(geminiResponse);

            saveResult = await _mediator.Send(
                request with { ResponseMessage = finalResponseJson }, cancellationToken);
        }


        var aggregateResult = ResultAggregator.AggregateWithNumbers(
            (geminiResult, false),
            (imageSearchResult, false),
            (saveResult, true)
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