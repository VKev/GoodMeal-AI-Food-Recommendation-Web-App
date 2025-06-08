using Application.Prompt.Commands;
using Application.Prompt.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Common;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class MessageController : ApiController
{
    public MessageController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateMessageCommand request,
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
        var result = await _mediator.Send(new GetAllMessageQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("read-active")]
    public async Task<IActionResult> GetAllActive(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllMessageActiveQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("read/{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMessageByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpGet("read-active/{promptSessionId}")]
    public async Task<IActionResult> GetMessageActiveById(Guid promptSessionId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMessageActiveByIdQuery(promptSessionId), cancellationToken);
        return Ok(result);
    }
    
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateMessageCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result);
    }

    [HttpDelete("soft-delete")]
    public async Task<IActionResult> SoftDelete([FromBody] SoftDeleteMessageCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromBody] DeleteMessageCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result);
    }
}