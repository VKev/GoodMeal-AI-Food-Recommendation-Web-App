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
        var result = await _mediator.Send(new GetAllPromptSessionQuery(), cancellationToken);
        var aggregateResult = ResultAggregator.AggregateWithNumbers(
            (result, true));
        if (aggregateResult.IsFailure)
        {
            return HandleFailure(aggregateResult);
        }

        return Ok(aggregateResult);
    }

    [HttpGet("read/{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPromptSessionByIdQuery(id), cancellationToken);
        var aggregateResult = ResultAggregator.AggregateWithNumbers(
            (result, true));
        if (aggregateResult.IsFailure)
        {
            return HandleFailure(aggregateResult);
        }

        return Ok(aggregateResult);
    }


    [HttpDelete("soft-delete")]
    public async Task<IActionResult> SoftDelete([FromBody] SoftDeletePromptSessionCommand request,
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
    public async Task<IActionResult> Delete([FromBody] DeletePromptSessionCommand request,
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