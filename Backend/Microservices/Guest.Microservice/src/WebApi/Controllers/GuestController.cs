using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Guests.Commands;
using Application.Guests.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Common;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class GuestController : ApiController
    {
        public GuestController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateGuestCommand request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            var aggregatedResult = ResultAggregator.AggregateWithNumbers(result);
            
            if (aggregatedResult.IsFailure)
            {
                return HandleFailure(aggregatedResult);
            }
            return Ok(aggregatedResult.Value);
        }

        [HttpGet("read")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllGuestsQuery(), cancellationToken);

            return Ok(result);
        }

        [HttpGet("health")]
        public async Task<IActionResult> Health()
        {
            
            
            return Ok();
        }
    }
}