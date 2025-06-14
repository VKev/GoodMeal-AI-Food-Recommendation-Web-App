using Application.Auths.Commands.AddUserRoleCommand;
using Application.Auths.Commands.RegisterUserCommand;
using Application.Auths.Commands.RemoveUserRoleCommand;
using Application.Auths.Commands.DisableUserCommand;
using Application.Auths.Commands.EnableUserCommand;
using Application.Auths.Commands.UpdateUserCommand;
using Application.Auths.Commands.DeleteUserCommand;
using Application.Auths.Queries.GetUserRolesQuery;
using Application.Auths.Queries.GetUserStatusQuery;
using Application.Auths.Queries.SearchUsersQuery;
using MediatR;
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

        [HttpGet("roles/{identityId}")]
        [ApiGatewayUser]
        public async Task<IActionResult> GetUserRoles(string identityId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetUserRolesQuery(identityId), cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result);
        }

        [HttpPost("add-role")]
        public async Task<IActionResult> AddRole([FromBody] AddUserRoleCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result);
        }

        [HttpPost("remove-role")]
        [ApiGatewayUser(Roles = "Admin")]
        public async Task<IActionResult> RemoveRole([FromBody] RemoveUserRoleCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result);
        }

        [HttpPost("disable")]
        [ApiGatewayUser(Roles = "Admin")]
        public async Task<IActionResult> DisableUser([FromBody] DisableUserCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result);
        }

        [HttpPost("enable")]
        [ApiGatewayUser(Roles = "Admin")]
        public async Task<IActionResult> EnableUser([FromBody] EnableUserCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result);
        }

        [HttpGet("status/{identityId}")]
        [ApiGatewayUser(Roles = "Admin")]
        public async Task<IActionResult> GetUserStatus(string identityId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetUserStatusQuery(identityId), cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result);
        }
        
        [HttpGet("users/search")]
        [ApiGatewayUser(Roles = "Admin")]
        public async Task<IActionResult> SearchUsers([FromQuery] string? searchTerm = null, [FromQuery] int pageSize = 50, [FromQuery] string? nextPageToken = null, CancellationToken cancellationToken = default)
        {
            var query = new SearchUsersQuery(searchTerm, pageSize, nextPageToken);
            var result = await _mediator.Send(query, cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result);
        }

        [HttpPut("users/{identityId}")]
        [ApiGatewayUser(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(string identityId, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateUserCommand(identityId, request.Email, request.DisplayName, request.EmailVerified);
            var result = await _mediator.Send(command, cancellationToken);
            if (result.IsFailure)
            {
                return HandleFailure(result);
            }

            return Ok(result);
        }

        [HttpDelete("users/{identityId}")]
        [ApiGatewayUser(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string identityId, CancellationToken cancellationToken)
        {
            var command = new DeleteUserCommand(identityId);
            var result = await _mediator.Send(command, cancellationToken);
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

    public class UpdateUserRequest
    {
        public string? Email { get; set; }
        public string? DisplayName { get; set; }
        public bool? EmailVerified { get; set; }
    }
}