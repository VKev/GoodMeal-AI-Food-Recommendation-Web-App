using Application.Foods.Commands;
using Application.Foods.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Common;
using SharedLibrary.Common.Messaging.Commands;
using SharedLibrary.Utils.AuthenticationExtention;
using ApiController = WebApi.Common.ApiController;

namespace WebApi.Controllers;

[Route("api/restaurant/[controller]")]
public class FoodController : ApiController
{
    public FoodController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost()]
    [ApiGatewayUser(Roles = "Business")]
    public async Task<IActionResult> Create([FromBody] CreateFoodCommand request, CancellationToken cancellationToken)
    {
        var createResult = await _mediator.Send(request, cancellationToken);
        var saveResult = await _mediator.Send(new SaveChangesCommand(), cancellationToken);
        var aggregatedResult = ResultAggregator.AggregateWithNumbers(
            (createResult, true), 
            (saveResult, false));
        if (aggregatedResult.IsFailure)
        {
            return HandleFailure(aggregatedResult);
        }

        return Ok(aggregatedResult.Value);
    }

    [HttpGet()]
    [ApiGatewayUser]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllFoodsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ApiGatewayUser]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetFoodByIdQuery(id), cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result);
    }

    [HttpPut()]
    [ApiGatewayUser(Roles = "Business")]
    public async Task<IActionResult> Update([FromBody] UpdateFoodCommand request, CancellationToken cancellationToken)
    {
        var updateResult = await _mediator.Send(request, cancellationToken);
        var saveResult = await _mediator.Send(new SaveChangesCommand(), cancellationToken);
        var aggregatedResult = ResultAggregator.AggregateWithNumbers(
            (updateResult, true), 
            (saveResult, false));
        if (aggregatedResult.IsFailure)
        {
            return HandleFailure(aggregatedResult);
        }

        return Ok(aggregatedResult.Value);
    }

    [HttpDelete()]
    [ApiGatewayUser(Roles = "Business")]
    public async Task<IActionResult> Delete([FromBody] DisableFoodCommand request, CancellationToken cancellationToken)
    {
        var deleteResult = await _mediator.Send(request, cancellationToken);
        var saveResult = await _mediator.Send(new SaveChangesCommand(), cancellationToken);
        var aggregatedResult = ResultAggregator.AggregateWithNumbers(
            (deleteResult, true), 
            (saveResult, false));
        if (aggregatedResult.IsFailure)
        {
            return HandleFailure(aggregatedResult);
        }

        return Ok(aggregatedResult.Value);
    }
}