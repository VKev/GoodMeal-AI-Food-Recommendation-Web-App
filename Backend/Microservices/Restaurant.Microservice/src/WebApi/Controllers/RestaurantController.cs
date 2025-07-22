using Application.Restaurants.Commands;
using Application.Restaurants.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Common;
using SharedLibrary.Common.Messaging.Commands;
using SharedLibrary.Utils.AuthenticationExtention;

namespace WebApi.Controllers;

[Route("api/restaurant/[controller]")]
public class RestaurantController : ApiController
{
    public RestaurantController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost()]
    [ApiGatewayUser(Roles = "Business")]
    public async Task<IActionResult> Create([FromBody] CreateRestaurantCommand request,
        CancellationToken cancellationToken)
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
        var result = await _mediator.Send(new GetAllRestaurantsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ApiGatewayUser]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetRestaurantByIdQuery(id), cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result);
    }

    [HttpPut()]
    [ApiGatewayUser(Roles = "Business")]
    public async Task<IActionResult> Update([FromBody] UpdateRestaurantCommand request,
        CancellationToken cancellationToken)
    {
        var updateResult = await _mediator.Send(request, cancellationToken);
        var saveResult = await _mediator.Send(new SaveChangesCommand(), cancellationToken);
        var aggregatedResult = ResultAggregator.AggregateWithNumbers(
            (updateResult, true),
            (saveResult, false));
        if (aggregatedResult.IsFailure)
            if (aggregatedResult.IsFailure)
            {
                return HandleFailure(aggregatedResult);
            }

        return Ok(aggregatedResult.Value);
    }

    [HttpDelete()]
    [ApiGatewayUser(Roles = "Business")]
    public async Task<IActionResult> Delete([FromBody] DisableRestaurantCommand request,
        CancellationToken cancellationToken)
    {
        var deleteResult = await _mediator.Send(request, cancellationToken);
        var saveResult = await _mediator.Send(new SaveChangesCommand(), cancellationToken);
        var aggregatedResult = ResultAggregator.AggregateWithNumbers(
            (deleteResult, true),
            (saveResult, false));
        if (aggregatedResult.IsFailure)
            if (aggregatedResult.IsFailure)
            {
                return HandleFailure(aggregatedResult);
            }

        return Ok(aggregatedResult.Value);
    }
}