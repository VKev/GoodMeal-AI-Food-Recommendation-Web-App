using Application.ElasticSearch.Commands;
using Application.ElasticSearch.Queries;
using Application.Restaurants.Commands;
using Application.Restaurants.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Common;
using SharedLibrary.Common.Messaging.Commands;
using SharedLibrary.Utils.AuthenticationExtention;

namespace WebApi.Controllers;

[Route("api/restaurant/[controller]")]
public class ElasticSearchController : ApiController
{
    public ElasticSearchController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost("create-index/{indexName}")]
    [ApiGatewayUser(Roles = "Admin")]
    public async Task<IActionResult> CreateIndex(string indexName, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateFoodIndexCommand(indexName), cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result);
    }

    [HttpPost("add-or-update")]
    [ApiGatewayUser(Roles = "Admin,Business")]
    public async Task<IActionResult> AddOrUpdateFood([FromBody] AddOrUpdateFoodCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        var saveResult = await _mediator.Send(new SaveChangesCommand(), cancellationToken);
        var aggregated = ResultAggregator.AggregateWithNumbers((result, true), (saveResult, false));
        if (aggregated.IsFailure)
        {
            return HandleFailure(aggregated);
        }

        return Ok(aggregated.Value);
    }

    [HttpPost("add-or-update-bulk")]
    [ApiGatewayUser(Roles = "Admin,Business")]
    public async Task<IActionResult> AddOrUpdateBulk([FromBody] AddOrUpdateFoodBulkCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        var saveResult = await _mediator.Send(new SaveChangesCommand(), cancellationToken);
        var aggregated = ResultAggregator.AggregateWithNumbers((result, true), (saveResult, false));
        if (aggregated.IsFailure)
        {
            return HandleFailure(aggregated);
        }

        return Ok(aggregated.Value);
    }

    [HttpDelete("remove-all")]
    [ApiGatewayUser(Roles = "Admin")]
    public async Task<IActionResult> RemoveAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RemoveAllFoodsCommand(), cancellationToken);
        var saveResult = await _mediator.Send(new SaveChangesCommand(), cancellationToken);
        var aggregated = ResultAggregator.AggregateWithNumbers((result, true), (saveResult, false));
        if (aggregated.IsFailure)
        {
            return HandleFailure(aggregated);
        }

        return Ok(aggregated.Value);
    }

    [HttpDelete("remove/{foodId}")]
    [ApiGatewayUser(Roles = "Admin")]
    public async Task<IActionResult> RemoveById(string foodId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RemoveFoodCommand(foodId), cancellationToken);
        var saveResult = await _mediator.Send(new SaveChangesCommand(), cancellationToken);
        var aggregated = ResultAggregator.AggregateWithNumbers((result, true), (saveResult, false));
        if (aggregated.IsFailure)
        {
            return HandleFailure(aggregated);
        }

        return Ok(aggregated.Value);
    }
    
    [HttpGet]
    [ApiGatewayUser]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllFoodQuery(), cancellationToken);
        if (result.IsFailure)
            return HandleFailure(result);

        return Ok(result.Value);
    }
    
    [HttpGet("{id:guid}")]
    [ApiGatewayUser]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetFoodByIdQuery(id.ToString()), cancellationToken);
        if (result.IsFailure)
            return HandleFailure(result);

        return Ok(result.Value);
    }
    
    [HttpGet("search/restaurants/{keyword}")]
    [ApiGatewayUser]
    public async Task<IActionResult> SearchRestaurantsByFood(string keyword, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SearchRestaurantByFoodNameQuery(keyword), cancellationToken);
        if (result.IsFailure)
            return HandleFailure(result);

        return Ok(result.Value);
    }
}
