using Application.Auths.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Common;
using SharedLibrary.Utils;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ApiController
    {
        public AuthController(IMediator mediator) : base(mediator)
        {

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpPost("login-with-external-provider")]
        public async Task<IActionResult> LoginGoogle([FromBody] LoginWithExternalProviderCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }
            return Ok(result);
        }
        
        [HttpGet("check-authorization")]
        [ApiGatewayUser]
        public IActionResult CheckAuthorization()
        {
            if (!User.Identity?.IsAuthenticated ?? false)
            {
                return Unauthorized(new { message = "Not authorized" });
            }

            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(new
            {
                message = "Authorized",
                user = new
                {
                    name = User.Identity.Name,
                    authenticationType = User.Identity.AuthenticationType,
                    claims
                }
            });
        }
        
        [HttpGet("health")]
        public async Task<IActionResult> Health()
        {
            return Ok();
        }
    }
}