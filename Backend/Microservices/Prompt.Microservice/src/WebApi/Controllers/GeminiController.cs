using System.Text.Json;
using Application.Prompt.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Common;

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
        var createMessageResult = await _mediator.Send(request with { ResponseMessage = geminiResult.Value }, cancellationToken);

        var aggregatedResult = ResultAggregator.Aggregate(geminiResult, createMessageResult);
        if (aggregatedResult.IsFailure)
        {
            return HandleFailure(aggregatedResult);
        }

        return Ok(geminiResult);
    }
}