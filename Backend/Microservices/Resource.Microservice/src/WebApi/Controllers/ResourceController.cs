using Application.Features.Aws.Commands;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Common;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ResourceController : ApiController
    {
        private readonly IMapper _mapper;
        public ResourceController(IMediator mediator, IMapper mapper) : base(mediator)
        {
            _mapper = mapper;
        }
        [HttpGet("health")]
        public IActionResult Health(CancellationToken cancellationToken)
        {
            return Ok();
        }

        [HttpPost("generate-presigned-url")]
        public async Task<IActionResult> GetPresignedUrl([FromBody] CreatePresignedUrlRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<CreatePresignedUrlCommand>(request);
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result); ;
            }

            return Ok(result);
        }

        [HttpPost("generate-temporary-credentials")]
        public async Task<IActionResult> GetTemporaryCredentials([FromBody] CreateTemporaryCredentialCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result); ;
            }

            return Ok(result);
        }
        [HttpPost("generate-signed-cookie")]
        public async Task<IActionResult> GetSignedCookie([FromBody] CreateCloudFrontSignedCookieCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result); ;
            }

            return Ok(result);
        }

    }
}