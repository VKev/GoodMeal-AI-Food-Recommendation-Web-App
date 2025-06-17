using Application.Auths.Commands.RegisterUserCommand;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Common;
using SharedLibrary.Utils.AuthenticationExtention;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ApiController
    {
        public AuthController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command,
            CancellationToken cancellationToken)
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
            var roles = User.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Role).Select(c => c.Value)
                .ToList();

            return Ok(new
            {
                message = "Authorized",
                user = new
                {
                    name = User.Identity.Name,
                    authenticationType = User.Identity.AuthenticationType,
                    userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                    email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value,
                    roles = roles,
                    allClaims = claims
                }
            });
        }

        [HttpGet("check-admin")]
        [ApiGatewayUser(Roles = "Admin")]
        public IActionResult CheckAdmin()
        {
            return Ok(new { message = "You have admin access!", timestamp = DateTime.UtcNow });
        }

        [HttpGet("check-user")]
        [ApiGatewayUser(Roles = "User,Admin")]
        public IActionResult CheckUser()
        {
            return Ok(new { message = "You have user access!", timestamp = DateTime.UtcNow });
        }
        
        [HttpGet("health")]
        public async Task<IActionResult> Health()
        {
            return Ok();
        }
    }
}