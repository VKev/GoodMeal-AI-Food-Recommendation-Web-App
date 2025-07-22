using Application.RestaurantRatings.Commands;
using Application.RestaurantRatings.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Common;
using SharedLibrary.Common.Messaging.Commands;
using SharedLibrary.Utils.AuthenticationExtention;
using ApiController = WebApi.Common.ApiController;

namespace WebApi.Controllers;

[Route("api/restaurant/[controller]")]
public class RatingController: ApiController
{
    public RatingController(IMediator mediator) : base(mediator)
    {
    }
    
    [HttpPost()]
    [ApiGatewayUser]
    public async Task<IActionResult> Create([FromBody] CreateRatingCommand request, CancellationToken cancellationToken)
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
        var result = await _mediator.Send(new GetAllRatingsQuery(), cancellationToken);
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    [ApiGatewayUser]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetRatingByIdQuery(id), cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }
        return Ok(result);
    }
    [HttpGet("restaurant/{id}")]
    [ApiGatewayUser]
    public async Task<IActionResult> GetByRestaurantId(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetRatingByRestaurantIdQuery(id), cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }
        return Ok(result);
    }
    [HttpGet("user/{id}")]
    [ApiGatewayUser]
    public async Task<IActionResult> GetByUserId(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetRatingByUserIdQuery(id), cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }
        return Ok(result);
    }
    [HttpPut()]
    [ApiGatewayUser]
    public async Task<IActionResult> Update([FromBody] UpdateRatingCommand request, CancellationToken cancellationToken)
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
    [ApiGatewayUser(Roles = "admin")]
    public async Task<IActionResult> Delete([FromBody] DisableRatingCommand request, CancellationToken cancellationToken)
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