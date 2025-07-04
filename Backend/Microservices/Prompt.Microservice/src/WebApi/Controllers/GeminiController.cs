using System.Text;
using System.Text.Json;
using Application.Common.GeminiApi;
using Application.Prompt.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharedLibrary.Common;
using SharedLibrary.Common.Messaging.Commands;
using SharedLibrary.Common.ResponseModel;
using SharedLibrary.Utils.AuthenticationExtention;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WebApi.Controllers;

[ApiController]
[Route("api/prompt/[controller]")]
public class GeminiController : ApiController
{
    private readonly ILogger<GeminiController> _logger;

    public GeminiController(IMediator mediator, ILogger<GeminiController> logger) : base(mediator)
    {
        _logger = logger;
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


    [HttpPost("process-food-request/v1")]
    [ApiGatewayUser]
    public async Task<IActionResult> ProcessFoodRequestAsync(
        [FromBody] CreateMessageCommand request,
        CancellationToken cancellationToken)
    {
        var geminiResult = await _mediator.Send(
            new CallGeminiCommand(request.PromptMessage), cancellationToken);

        var cleanedJson = CleanGeminiResponse.CleanResponse(geminiResult.Value);

        var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(
            cleanedJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (geminiResponse is null)
            return BadRequest("Failed to parse Gemini response.");

        var imageSearchResult = await _mediator.Send(
            new GoogleImageSearchCommand(geminiResponse.FoodNames), cancellationToken);

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
            (saveResult, true),
            (save, false)
        );

        if (aggregateResult.IsFailure)
            return HandleFailure(aggregateResult);

        return Ok(new
        {
            AggregatedResults = aggregateResult.Value,
            FinalResponse = geminiResponse
        });
    }


    [HttpPost("process-food-request/v2")]
    [ApiGatewayUser]
    public async Task<IActionResult> ProcessFoodRequestAsyncV2(
        [FromBody] CreateMessageCommand request,
        CancellationToken cancellationToken)
    {
        var geminiResult = await _mediator.Send(new CallGeminiCommand(request.PromptMessage), cancellationToken);

        var cleanedJson = CleanGeminiResponse.CleanResponse(geminiResult.Value);

        var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(
            cleanedJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var imageSearchResult = await _mediator.Send(
            new RapidApiImageSearchCommand(geminiResponse!.FoodNames), cancellationToken);

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
        var saveChangesResult = await _mediator.Send(new SaveChangesCommand(), cancellationToken);

        var aggregateResult = ResultAggregator.AggregateWithNumbers(
            (geminiResult, false),
            (imageSearchResult, false),
            (saveResult, true),
            (saveChangesResult, false)
        );

        if (aggregateResult.IsFailure)
            return HandleFailure(aggregateResult);

        return Ok(new
        {
            AggregatedResults = aggregateResult.Value,
            FinalResponse = geminiResponse
        });
    }

    [HttpPost("process-food-request/v3")]
    [ApiGatewayUser]
    public async Task<IActionResult> ProcessFoodRequestAsyncV3(
        [FromBody] CreateMessageCommand request,
        CancellationToken cancellationToken)
    {
        var geminiResult = await _mediator.Send(new CallGeminiCommand(request.PromptMessage), cancellationToken);

        var cleanedJson = CleanGeminiResponse.CleanResponse(geminiResult.Value);

        var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(
            cleanedJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var imageSearchResult = await _mediator.Send(
            new GoogleImageSearchWithVisionApiCommand(geminiResponse!.FoodNames),
            cancellationToken);

        foreach (var food in geminiResponse.Foods)
        {
            if (imageSearchResult.Value.TryGetValue(food.FoodName, out var imageUrl))
            {
                food.ImageUrl = imageUrl;
            }
        }

        var finalResponseJson = JsonSerializer.Serialize(geminiResponse);

        var saveResult = await _mediator.Send(
            request with { ResponseMessage = finalResponseJson },
            cancellationToken);

        var saveChangesResult = await _mediator.Send(new SaveChangesCommand(), cancellationToken);

        var aggregateResult = ResultAggregator.AggregateWithNumbers(
            (geminiResult, false),
            (imageSearchResult, false),
            (saveResult, true),
            (saveChangesResult, false)
        );

        if (aggregateResult.IsFailure)
            return HandleFailure(aggregateResult);

        return Ok(new
        {
            AggregatedResults = aggregateResult.Value,
            FinalResponse = geminiResponse
        });
    }

    [HttpPost("process-food-request/stream")]
    [ApiGatewayUser]
    public async Task ProcessFoodRequestStreamAsync([FromBody] CreateMessageCommand request,
        CancellationToken cancellationToken)
    {
        Response.ContentType = "text/event-stream";
        await using var writer = new StreamWriter(Response.Body);

        var geminiResult = await _mediator.Send(new CallGeminiStreamCommand(request.PromptMessage), cancellationToken);
        if (geminiResult.IsFailure)
        {
            await writer.WriteAsync($"event: error\ndata: {geminiResult.Error}\n\n");
            await writer.FlushAsync(cancellationToken);
            return;
        }

        var geminiResponse = geminiResult.Value;
        var titleMessage = new
        {
            geminiResponse.Value.Title, geminiResponse.Value.Error
        };
        var messageJson = JsonSerializer.Serialize(titleMessage);
        await writer.WriteAsync($"data: {messageJson}\n\n");
        await writer.FlushAsync(cancellationToken);
        foreach (var food in geminiResponse.Value.Foods)
        {
            _logger.LogInformation("🔍 Đang xử lý ảnh cho món: {FoodName}", food.FoodName);

            var result = await _mediator.Send(new GoogleImageSearchStreamCommand(new List<string> { food.FoodName }),
                cancellationToken);

            if (result.IsSuccess && result.Value.TryGetValue(food.FoodName, out var imageUrl))
            {
                food.ImageUrl = imageUrl;

                var message = JsonSerializer.Serialize(new
                {
                    food.FoodName,
                    food.National,
                    food.Description,
                    food.ImageUrl
                });

                _logger.LogInformation("📤 Streaming response to client: {StreamMessage}", message);

                await writer.WriteAsync($"data: {message}\n\n");
                await writer.FlushAsync(cancellationToken);
            }
            else
            {
                _logger.LogWarning("❌ Không tìm thấy ảnh cho món: {FoodName}", food.FoodName);
            }
        }

        var finalResponse = new
        {
            geminiResponse.Value.Title,
            geminiResponse.Value.Error,
            geminiResponse.Value.Location,
            Foods = geminiResponse.Value.Foods.Select(f => new
            {
                f.FoodName,
                f.National,
                f.Description,
                f.ImageUrl
            }).ToList()
        };

        var finalResponseJson = JsonSerializer.Serialize(finalResponse);
        await _mediator.Send(
            request with { ResponseMessage = finalResponseJson }, cancellationToken);

        var locationMessage = JsonSerializer.Serialize(new { geminiResponse.Value.Location });
        _logger.LogInformation("📤 Streaming location to client: {LocationMessage}", locationMessage);
        await writer.WriteAsync($"event: done\ndata: {locationMessage}\n\n");
        await writer.FlushAsync(cancellationToken);
        await _mediator.Send(new SaveChangesCommand(), cancellationToken);
        _logger.LogInformation("🏁 Đã hoàn tất stream cho request prompt: {Prompt}", request.PromptMessage);
    }
}