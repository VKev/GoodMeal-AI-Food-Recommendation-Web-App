using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Prompt.Commands;
using Application.Prompt.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Common;
using SharedLibrary.Common.Messaging.Commands;

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
        var save = await _mediator.Send(new SaveChangesCommand(), cancellationToken);
        var aggregateResult = ResultAggregator.AggregateWithNumbers(
            (result, true),
            (save, false));
        if (aggregateResult.IsFailure)
        {
            return HandleFailure(aggregateResult);
        }

        return Ok(aggregateResult);
    }

    [HttpGet("read")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllMessageQuery(), cancellationToken);
        var save = await _mediator.Send(new SaveChangesCommand(), cancellationToken);
        var aggregateResult = ResultAggregator.AggregateWithNumbers(
            (result, true),
            (save, false));
        if (aggregateResult.IsFailure)
        {
            return HandleFailure(aggregateResult);
        }

        return Ok(aggregateResult);
    }

    [HttpGet("read-active")]
    public async Task<IActionResult> GetAllActive(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllMessageActiveQuery(), cancellationToken);
        var save = await _mediator.Send(new SaveChangesCommand(), cancellationToken);
        var aggregateResult = ResultAggregator.AggregateWithNumbers(
            (result, true),
            (save, false));
        if (aggregateResult.IsFailure)
        {
            return HandleFailure(aggregateResult);
        }

        return Ok(aggregateResult);
    }

    [HttpGet("read/{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMessageByIdQuery(id), cancellationToken);
        var save = await _mediator.Send(new SaveChangesCommand(), cancellationToken);
        var aggregateResult = ResultAggregator.AggregateWithNumbers(
            (result, true),
            (save, false));
        if (aggregateResult.IsFailure)
        {
            return HandleFailure(aggregateResult);
        }

        return Ok(aggregateResult);
    }

    [HttpGet("read-active/{promptSessionId}")]
    public async Task<IActionResult> GetMessageActiveById(Guid promptSessionId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMessageActiveByIdQuery(promptSessionId), cancellationToken);
        var save = await _mediator.Send(new SaveChangesCommand(), cancellationToken);
        var aggregateResult = ResultAggregator.AggregateWithNumbers(
            (result, true),
            (save, false));
        if (aggregateResult.IsFailure)
        {
            return HandleFailure(aggregateResult);
        }

        return Ok(aggregateResult);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateMessageCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        var save = await _mediator.Send(new SaveChangesCommand(), cancellationToken);
        var aggregateResult = ResultAggregator.AggregateWithNumbers(
            (result, true),
            (save, false));
        if (aggregateResult.IsFailure)
        {
            return HandleFailure(aggregateResult);
        }

        return Ok(aggregateResult);
    }

    [HttpDelete("soft-delete")]
    public async Task<IActionResult> SoftDelete([FromBody] SoftDeleteMessageCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        var save = await _mediator.Send(new SaveChangesCommand(), cancellationToken);
        var aggregateResult = ResultAggregator.AggregateWithNumbers(
            (result, true),
            (save, false));
        if (aggregateResult.IsFailure)
        {
            return HandleFailure(aggregateResult);
        }

        return Ok(aggregateResult);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromBody] DeleteMessageCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        var save = await _mediator.Send(new SaveChangesCommand(), cancellationToken);
        var aggregateResult = ResultAggregator.AggregateWithNumbers(
            (result, true),
            (save, false));
        if (aggregateResult.IsFailure)
        {
            return HandleFailure(aggregateResult);
        }

        return Ok(aggregateResult);
    }
}