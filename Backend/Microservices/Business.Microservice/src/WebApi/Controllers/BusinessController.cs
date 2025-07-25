using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Business.Queries.GetAllBusinessesQuery;
using Application.Business.Queries.GetBusinessByIdQuery;
using Application.Business.Queries.GetMyBusinessQuery;
using Application.Business.Queries.GetBusinessRestaurantsQuery;
using Application.Business.Commands.CreateBusinessCommand;
using Application.Business.Commands.UpdateBusinessCommand;
using Application.Business.Commands.DisableBusinessCommand;
using Application.Business.Commands.EnableBusinessCommand;
using Application.Business.Commands.ActiveBusinessCommand;
using Application.Business.Commands.InactiveBusinessCommand;
using Application.Business.Commands.AddRestaurantToBusinessCommand;
using SharedLibrary.Common;
using SharedLibrary.Common.Messaging.Commands;
using SharedLibrary.Utils.AuthenticationExtention;

namespace WebApi.Controllers;

[Route("api/[controller]")]
public class BusinessController : ApiController
{
    public BusinessController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet]
    [ApiGatewayUser]
    public async Task<IActionResult> GetAllBusinesses(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllBusinessesQuery(), cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result);
    }

    [HttpGet("{businessId}")]
    [ApiGatewayUser]
    public async Task<IActionResult> GetBusinessById(Guid businessId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetBusinessByIdQuery(businessId), cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result);
    }

    [HttpGet("my-business")]
    [ApiGatewayUser]
    public async Task<IActionResult> GetMyBusiness(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMyBusinessQuery(), cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result);
    }

    [HttpGet("{businessId}/restaurants")]
    [ApiGatewayUser]
    public async Task<IActionResult> GetBusinessRestaurants(Guid businessId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetBusinessRestaurantsQuery(businessId), cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result);
    }

    [HttpPost]
    [ApiGatewayUser]
    public async Task<IActionResult> CreateBusiness([FromBody] CreateBusinessCommand command,
        CancellationToken cancellationToken)
    {
        var createResult = await _mediator.Send(command, cancellationToken);
        var saveResult = await _mediator.Send(new SaveChangesCommand(), cancellationToken);
        var getBusinessResult = await _mediator.Send(new GetBusinessByIdQuery(createResult.Value?.Id ?? Guid.Empty),
            cancellationToken);

        var aggregatedResult = ResultAggregator.AggregateWithNumbers(
            (createResult, false),
            (saveResult, false),
            (getBusinessResult, true)
        );

        if (aggregatedResult.IsFailure)
        {
            return HandleFailure(aggregatedResult);
        }

        return Ok(aggregatedResult.Value);
    }

    [HttpPut("{businessId}")]
    [ApiGatewayUser]
    public async Task<IActionResult> UpdateBusiness(Guid businessId, [FromBody] UpdateBusinessRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateBusinessCommand(
            businessId,
            request.Name,
            request.Description,
            request.Address,
            request.Phone,
            request.Email,
            request.Website
        );

        var updateResult = await _mediator.Send(command, cancellationToken);
        var saveResult = await _mediator.Send(new SaveChangesCommand(), cancellationToken);
        var getBusinessResult = await _mediator.Send(new GetBusinessByIdQuery(businessId), cancellationToken);

        var aggregatedResult = ResultAggregator.AggregateWithNumbers(
            (updateResult, false),
            (saveResult, false),
            (getBusinessResult, true)
        );

        if (aggregatedResult.IsFailure)
        {
            return HandleFailure(aggregatedResult);
        }

        return Ok(aggregatedResult.Value);
    }

    [HttpDelete("{businessId}")]
    [ApiGatewayUser]
    public async Task<IActionResult> DisableBusiness(Guid businessId, CancellationToken cancellationToken)
    {
        var disableResult = await _mediator.Send(new DisableBusinessCommand(businessId), cancellationToken);
        var saveResult = await _mediator.Send(new SaveChangesCommand(), cancellationToken);
        var getBusinessResult = await _mediator.Send(new GetBusinessByIdQuery(businessId), cancellationToken);

        var aggregatedResult = ResultAggregator.AggregateWithNumbers(
            (disableResult, false),
            (saveResult, false),
            (getBusinessResult, true)
        );

        if (aggregatedResult.IsFailure)
        {
            return HandleFailure(aggregatedResult);
        }

        return Ok(aggregatedResult.Value);
    }

    [HttpPost("{businessId}/enable")]
    [ApiGatewayUser]
    public async Task<IActionResult> EnableBusiness(Guid businessId, CancellationToken cancellationToken)
    {
        var enableResult = await _mediator.Send(new EnableBusinessCommand(businessId), cancellationToken);
        var saveResult = await _mediator.Send(new SaveChangesCommand(), cancellationToken);
        var getBusinessResult = await _mediator.Send(new GetBusinessByIdQuery(businessId), cancellationToken);

        var aggregatedResult = ResultAggregator.AggregateWithNumbers(
            (enableResult, false),
            (saveResult, false),
            (getBusinessResult, true)
        );

        if (aggregatedResult.IsFailure)
        {
            return HandleFailure(aggregatedResult);
        }

        return Ok(aggregatedResult.Value);
    }

    [HttpPost("{businessId}/activate")]
    [ApiGatewayUser]
    public async Task<IActionResult> ActivateBusiness(Guid businessId, CancellationToken cancellationToken)
    {
        var activateResult = await _mediator.Send(new ActiveBusinessCommand(businessId), cancellationToken);
        var saveResult = await _mediator.Send(new SaveChangesCommand(), cancellationToken);
        var getBusinessResult = await _mediator.Send(new GetBusinessByIdQuery(businessId), cancellationToken);

        var aggregatedResult = ResultAggregator.AggregateWithNumbers(
            (activateResult, false),
            (saveResult, false),
            (getBusinessResult, true)
        );

        if (aggregatedResult.IsFailure)
        {
            return HandleFailure(aggregatedResult);
        }

        return Ok(aggregatedResult.Value);
    }

    [HttpPost("{businessId}/deactivate")]
    [ApiGatewayUser]
    public async Task<IActionResult> DeactivateBusiness(Guid businessId, CancellationToken cancellationToken)
    {
        var deactivateResult = await _mediator.Send(new InactiveBusinessCommand(businessId), cancellationToken);
        var saveResult = await _mediator.Send(new SaveChangesCommand(), cancellationToken);
        var getBusinessResult = await _mediator.Send(new GetBusinessByIdQuery(businessId), cancellationToken);

        var aggregatedResult = ResultAggregator.AggregateWithNumbers(
            (deactivateResult, false),
            (saveResult, false),
            (getBusinessResult, true)
        );

        if (aggregatedResult.IsFailure)
        {
            return HandleFailure(aggregatedResult);
        }

        return Ok(aggregatedResult.Value);
    }

    [HttpPost("{businessId}/restaurants")]
    [ApiGatewayUser]
    public async Task<IActionResult> AddRestaurantToBusiness(Guid businessId,
        [FromBody] AddRestaurantRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddRestaurantToBusinessCommand(
            businessId,
            request.Name,
            request.Address,
            request.Phone
        );

        var addResult = await _mediator.Send(command, cancellationToken);
        var saveResult = await _mediator.Send(new SaveChangesCommand(), cancellationToken);
        var getRestaurantsResult = await _mediator.Send(new GetBusinessRestaurantsQuery(businessId), cancellationToken);

        var aggregatedResult = ResultAggregator.AggregateWithNumbers(
            (addResult, true),
            (saveResult, false),
            (getRestaurantsResult, true)
        );

        if (aggregatedResult.IsFailure)
        {
            return HandleFailure(aggregatedResult);
        }

        return Ok(aggregatedResult.Value);
    }

    [HttpGet("health")]
    public async Task<IActionResult> Health()
    {
        return Ok();
    }
}