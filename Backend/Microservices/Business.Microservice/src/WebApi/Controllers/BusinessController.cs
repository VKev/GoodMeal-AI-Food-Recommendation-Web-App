using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Common;
using Application.Business.Queries.GetAllBusinessesQuery;
using Application.Business.Queries.GetBusinessByIdQuery;
using Application.Business.Queries.GetMyBusinessQuery;
using Application.Business.Queries.GetBusinessRestaurantsQuery;
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

    [HttpGet("health")]
    public async Task<IActionResult> Health()
    {
        return Ok();
    }
} 