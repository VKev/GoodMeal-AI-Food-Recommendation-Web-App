using Application.SearchPlaces.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Common;

namespace WebApi.Controllers;

[Route("api/restaurant/[controller]")]
public class PlaceController: ApiController
{
    public PlaceController(IMediator mediator) : base(mediator)
    {
    }
    
    //FourSquare API
    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchPlacesCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SearchPlacesCommand(request.Query, request.Latitude, request.Longitude), cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }
        return Ok(result);
    }
    
    [HttpPost("nearby")]
    public async Task<IActionResult> GetNearbyPlaces([FromBody] SearchNearbyPlacesCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SearchNearbyPlacesCommand(request.Query, request.Lat, request.Lng, request.Limit, request.Offset, request.Country, request.Lang, request.Zoom), cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }
        return Ok(result);
    }
    
    [HttpPost("detail")]
    public async Task<IActionResult> GetPlaceDetail([FromBody] SearchPlaceDetailCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SearchPlaceDetailCommand(request.Business_id, request.Place_id, request.Country, request.Lang), cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }
        return Ok(result);
    }
    
    [HttpPost("geocoding")]
    public async Task<IActionResult> GetGeocodingAddress([FromBody] SearchGeocodingAddressCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SearchGeocodingAddressCommand(request.Query, request.Lang, request.Country), cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }
        return Ok(result);
    }
    
    [HttpPost("review")]
    public async Task<IActionResult> GetPlaceReviews([FromBody] SearchPlaceReviewsCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SearchPlaceReviewsCommand(request.Business_id, request.Place_id, request.Country, request.Lang, request.limit, request.cursor, request.sort), cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }
        return Ok(result);
    }
    
    [HttpPost("photo")]
    public async Task<IActionResult> GetPlacePhotos([FromBody] SearchPlacePhotosCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SearchPlacePhotosCommand(request.Business_id, request.Place_id, request.Country, request.Lang, request.cursor), cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }
        return Ok(result);
    }
}