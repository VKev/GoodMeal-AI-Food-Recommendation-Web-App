using Application.Prompt.Commands;
using Application.Prompt.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Common;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class PromptSessionController : ApiController
{
    public PromptSessionController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreatePromptSessionCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result);
    }

    [HttpGet("read")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllPromptSessionQuery(), cancellationToken);
        return Ok(result);
    }
}