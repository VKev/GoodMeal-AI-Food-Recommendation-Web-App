using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Common;
using Application.Business.Queries.GetAllBusinessesQuery;
using Application.Business.Queries.GetBusinessByIdQuery;
using Application.Business.Queries.GetMyBusinessQuery;
using Application.Business.Queries.GetBusinessRestaurantsQuery;
using Application.Business.Commands.CreateBusinessCommand;
using Application.Business.Commands.UpdateBusinessCommand;
using Application.Business.Commands.DisableBusinessCommand;
using Application.Business.Commands.EnableBusinessCommand;
using Application.Business.Commands.AddRestaurantToBusinessCommand;
using SharedLibrary.Utils;
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
        var result = await _mediator.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return CreatedAtAction(nameof(GetBusinessById), new { businessId = result.Value.Id }, result);
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

        var result = await _mediator.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result);
    }

    [HttpDelete("{businessId}")]
    [ApiGatewayUser]
    public async Task<IActionResult> DisableBusiness(Guid businessId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DisableBusinessCommand(businessId), cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result);
    }

    [HttpPatch("{businessId}/enable")]
    [ApiGatewayUser]
    public async Task<IActionResult> EnableBusiness(Guid businessId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new EnableBusinessCommand(businessId), cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result);
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

        var result = await _mediator.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        // Trigger SaveChanges to flush events
        await _mediator.Send(new SharedLibrary.Common.Messaging.Commands.SaveChangesCommand(), cancellationToken);

        return Accepted(result);
    }

    [HttpGet("health")]
    public async Task<IActionResult> Health()
    {
        return Ok();
    }
}