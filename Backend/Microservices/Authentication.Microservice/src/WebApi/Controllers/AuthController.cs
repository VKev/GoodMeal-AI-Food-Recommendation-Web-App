using Application.Users.Commands;
using Application.Users.Queries;
using Application.Auths.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Common;

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
        public async Task<IActionResult> Login([FromBody] LoginUserQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
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

        [HttpPost("login-google")]
        public async Task<IActionResult> LoginGoogle([FromBody] LoginWithGoogleQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }
            return Ok(result);
        }

        [HttpPost("login-facebook")]
        public async Task<IActionResult> LoginFacebook([FromBody] LoginWithFacebookQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }
            return Ok(result);
        }

        [HttpPost("login-github")]
        public async Task<IActionResult> LoginGitHub([FromBody] LoginWithGitHubQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
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
}