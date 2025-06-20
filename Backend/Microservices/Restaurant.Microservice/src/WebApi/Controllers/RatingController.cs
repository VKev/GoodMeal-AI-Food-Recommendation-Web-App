using Application.RestaurantRatings.Commands;
using Application.RestaurantRatings.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Common;

namespace WebApi.Controllers;

[Route("api/restaurant/[controller]")]
public class RatingController: ApiController
{
    public RatingController(IMediator mediator) : base(mediator)
    {
    }
    
    [HttpPost()]
    public async Task<IActionResult> Create([FromBody] CreateRatingCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }
        return Ok(result);
    }
    [HttpGet()]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllRatingsQuery(), cancellationToken);
        return Ok(result);
    }
    
    [HttpGet("{id}")]
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
    public async Task<IActionResult> Update([FromBody] UpdateRatingCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }
        return Ok(result);
    }
    [HttpDelete()]
    public async Task<IActionResult> Delete([FromBody] DisableRatingCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }
        return Ok(result);
    }
}